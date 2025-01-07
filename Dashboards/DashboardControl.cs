using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Shapes;

namespace DragNDropTask.Dashboards
{
    [TemplatePart(Name = DashboardRootName, Type = typeof(Grid))]
    public class DashboardControl : Control
    {
        private const string DashboardRootName = "DashboardRoot";

        public static readonly DependencyProperty LayoutSettingProperty
            = DependencyProperty.Register(nameof(LayoutSetting), typeof(LayoutSetting), typeof(DashboardControl),
                new PropertyMetadata(null, OnLayoutSettingChanged));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource), typeof(IEnumerable), typeof(DashboardControl),
            new PropertyMetadata(default(IEnumerable), OnItemsSourceChanged));
        

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate), typeof(DataTemplate), typeof(DashboardControl),
            new PropertyMetadata(default(DataTemplate)));

        public delegate void PositionsSwappedRoutedEventHandler(object sender, PositionsSwappedRoutedEventArgs routedEventArgs);
        
        public static readonly RoutedEvent PositionsSwappedRoutedEvent 
            = EventManager.RegisterRoutedEvent(
                nameof(PositionsSwapped), 
                RoutingStrategy.Bubble, 
                typeof(PositionsSwappedRoutedEventHandler), 
                typeof(DashboardControl));


        public event PositionsSwappedRoutedEventHandler PositionsSwapped
        {
            add => AddHandler(PositionsSwappedRoutedEvent, value);
            remove => RemoveHandler(PositionsSwappedRoutedEvent, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public LayoutSetting LayoutSetting
        {
            get => (LayoutSetting)GetValue(LayoutSettingProperty);
            set => SetValue(LayoutSettingProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private Grid? DashboardRoot { get; set; }

        private DragDropHelper? DragDropHelper { get; set; }

        private Dictionary<UIElement, WidgetViewModel> ContentItemsDictionary { get; set; } = new();



        private static void OnLayoutSettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DashboardControl dashboardControl)
            {
                dashboardControl.HandleLayoutSettingsChanged(e);
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DashboardControl dashboardControl)
            {
                dashboardControl.HandleItemsSourceChanged(e);
            }
        }

        private void HandleItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue is ObservableCollection<WidgetViewModel> observableCollection)
            {
                observableCollection.CollectionChanged += HandleItemsCollectionChanged;
            }

            if (e.OldValue is ObservableCollection<WidgetViewModel> deletedObservableCollection)
            {
                deletedObservableCollection.CollectionChanged -= HandleItemsCollectionChanged;
            }
        }

        private void HandleItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is WidgetViewModel widgetViewModel)
                    {
                        AddElementToDashboard(widgetViewModel);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is WidgetViewModel widgetViewModel)
                    {
                        RemoveElementFromDashboard(widgetViewModel);
                    }
                }
            }

            ChangeItemsPositions();

        }

        private void ChangeItemsPositions()
        {
            if (DashboardRoot == null)
            {
                return;
            }

            var itemEnumerator = ItemsSource.GetEnumerator();
            while (itemEnumerator.MoveNext())
            {

                if (itemEnumerator.Current is not WidgetViewModel widgetViewModel)
                {
                    continue;
                }

                var widgetPosition = GetPositionsByLayoutSettings().ElementAtOrDefault(widgetViewModel.PosIndex);

                if (widgetPosition == null)
                {
                    return;
                }

                ContentItemsDictionary
                    .FirstOrDefault(pair => pair.Value == widgetViewModel).Key?
                    .SetWidgetPositionOnDashboard(widgetPosition);
            }

            if (itemEnumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }


        private void HandleLayoutSettingsChanged(DependencyPropertyChangedEventArgs ea)
        {
            if (DashboardRoot == null)
            {
                return;
            }

            ClearCurrentLayoutInformation();

            var positions = GetPositionsByLayoutSettings();

            int rowCount = positions.Max(pos => pos.Row);
            for (int i = 0; i <= rowCount; i++)
            {
                DashboardRoot.RowDefinitions.Add(new RowDefinition());
            }

            int columnCount = positions.Max(pos => pos.Column);
            for (int i = 0; i <= columnCount; i++)
            {
                DashboardRoot.ColumnDefinitions.Add(new ColumnDefinition());
            }

            ChangeItemsPositions();
        }

        private void HandleAfterDrop(UIElement sourceElement, UIElement targetElement)
        {
            var positions = GetPositionsByLayoutSettings();
            var sourceElementPosition = WidgetPosition.GetWidgetPositionByElement(sourceElement);
            var targetElementPosition = WidgetPosition.GetWidgetPositionByElement(targetElement);
            var sourceElementIndex = Array.FindIndex(positions, p => p.Equals(sourceElementPosition));
            var targetElementIndex = Array.FindIndex(positions, p => p.Equals(targetElementPosition));
            
            
            RaiseEvent(new PositionsSwappedRoutedEventArgs(PositionsSwappedRoutedEvent, this)
            {
                Positions = new(sourceElementIndex, targetElementIndex)
            });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild(DashboardRootName) is not Grid dashboardRoot)
            {
                throw new ArgumentException($"Unable to find grid with '{DashboardRootName}' name");
            }


            DashboardRoot = dashboardRoot;
            DragDropHelper = new DragDropHelper(DashboardRoot, HandleAfterDrop);
            PopulateRoot();
        }

        private void PopulateRoot()
        {
            foreach (var item in ItemsSource)
            {
                if (item is WidgetViewModel widgetViewModel)
                {
                    AddElementToDashboard(widgetViewModel);
                }
            }
        }

        private void ClearCurrentLayoutInformation()
        {
            if (DashboardRoot == null)
            {
                return;
            }

            DashboardRoot.RowDefinitions.Clear();
            DashboardRoot.ColumnDefinitions.Clear();
        }

        private void AddElementToDashboard(WidgetViewModel widgetViewModel)
        {
            if (DashboardRoot == null)
            {
                return;
            }

            ContentControl contentControl = new();

            DragDropHelper?.Register(contentControl);

            if (widgetViewModel.Content is UIElement element)
            {
                contentControl.Content = element;
            }
            else
            {
                contentControl.Content = new TextBlock()
                {
                    Text = "NULL VIEWMODEL CONTENT'S VALUE"
                };
            }

            Binding itemTemplateBinding = new(nameof(ItemTemplate))
            {
                Source = this
            };
            contentControl.SetBinding(ContentControl.ContentTemplateProperty, itemTemplateBinding);

            DashboardRoot.Children.Add(contentControl);
            ContentItemsDictionary.Add(contentControl, widgetViewModel);
        }


        private void RemoveElementFromDashboard(WidgetViewModel widgetViewModel)
        {
            var elementToDelete =
                ContentItemsDictionary.FirstOrDefault((pair => pair.Value == widgetViewModel)).Key;
            if (elementToDelete != null)
            {
                DragDropHelper?.Unregister(elementToDelete);
                DashboardRoot?.Children.Remove(elementToDelete);
                ContentItemsDictionary.Remove(elementToDelete);
            }
        }

        private WidgetPosition[] GetPositionsByLayoutSettings()
        {
            Dictionary<int, WidgetPosition> positions = new();
            for (int i = 0; i < LayoutSetting.RowCount; i++)
            {
                for (int j = 0; j < LayoutSetting.ColumnCount; j++)
                {
                    int currentElemIndex = LayoutSetting.ElementsScheme[i, j];
                    if (positions.ContainsKey(currentElemIndex) || currentElemIndex == 0)
                    {
                        continue;
                    }

                    var widgetPosition = new WidgetPosition()
                    {
                        Row = i,
                        Column = j,
                    };

                    int k = 1;
                    while (i + k < LayoutSetting.RowCount)
                    {
                        if (LayoutSetting.ElementsScheme[i + k, j] != currentElemIndex)
                        {
                            break;
                        }

                        k++;
                    }

                    widgetPosition.RowSpan = k;

                    k = 1;
                    while (j + k < LayoutSetting.ColumnCount)
                    {
                        if (LayoutSetting.ElementsScheme[i, j + k] != currentElemIndex)
                        {
                            break;
                        }

                        k++;
                    }

                    widgetPosition.ColumnSpan = k;

                    positions.Add(currentElemIndex, widgetPosition);
                }
            }

            return positions.Values.ToArray();
        }
    }

    internal sealed class WidgetPosition
    {
        public int Row { get; init; }

        public int Column { get; init; }

        public int RowSpan { get; set; } = 1;

        public int ColumnSpan { get; set; } = 1;

        public static WidgetPosition GetWidgetPositionByElement(UIElement element)
        {
            var row = Grid.GetRow(element);
            var column = Grid.GetColumn(element);
            var rowSpan = Grid.GetRowSpan(element);
            var columnSpan = Grid.GetColumnSpan(element);
            return new WidgetPosition()
            {
                Row = row,
                Column = column,
                RowSpan = rowSpan,
                ColumnSpan = columnSpan
            };
        }

        public override bool Equals(object? obj)
        {
            if (obj is not WidgetPosition wp)
            {
                return false;
            }

            return wp.Row == Row && wp.Column == Column && wp.RowSpan == RowSpan && wp.ColumnSpan == ColumnSpan;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column, RowSpan, ColumnSpan);
        }
    }
}
