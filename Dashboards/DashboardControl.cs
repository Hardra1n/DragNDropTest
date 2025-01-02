using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DragNDropTask.Dashboards
{
    [TemplatePart(Name = DashboardRootName, Type =  typeof(Grid))]
    public class DashboardControl : Control
    {
        private const string DashboardRootName = "DashboardRoot";

        public static readonly DependencyProperty LayoutSettingProperty
            = DependencyProperty.Register(nameof(LayoutSetting), typeof(LayoutSetting), typeof(DashboardControl),
                new PropertyMetadata(null, OnLayoutSettingChanged));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource), typeof(IEnumerable), typeof(DashboardControl), new PropertyMetadata(default(IEnumerable)));

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate), typeof(DataTemplate), typeof(DashboardControl), new PropertyMetadata(default(DataTemplate)));

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



        private static void OnLayoutSettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DashboardControl dashboardControl)
            {
                dashboardControl.HandleLayoutSettingsChanged(e);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild(DashboardRootName) is not Grid dashboardRoot)
            {
                throw new ArgumentException($"Unable to find grid with '{DashboardRootName}' name");
            }


            DashboardRoot = dashboardRoot;
            DragDropHelper = new DragDropHelper(DashboardRoot);
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

            var itemEnumerator = ItemsSource.GetEnumerator();
            for (int i = 0; i < positions.Length && itemEnumerator.MoveNext(); i++)
            {
                var item = itemEnumerator.Current;
                if (item != null)
                {
                    AddElementToDashboard(positions[i], item);
                }
            }

            if (itemEnumerator is IDisposable disposable)
            {
                disposable.Dispose();
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
            foreach (var child in DashboardRoot.Children)
            {
                if (child is UIElement element)
                {
                    DragDropHelper?.Unregister(element);
                }
            }
            DashboardRoot.Children.RemoveRange(0, DashboardRoot.Children.Count);
        }

        private void AddElementToDashboard(WidgetPosition position, object item)
        {
            if (DashboardRoot == null || 
                item is not WidgetViewModel { Content: UIElement element })
            {
                return;
            }

            ContentControl contentControl = new();

            DragDropHelper?.Register(contentControl);
            contentControl.SetWidgetPositionOnDashboard(position);
            contentControl.Content = element;

            Binding itemTemplateBinding = new(nameof(ItemTemplate))
            {
                Source = this
            };
            contentControl.SetBinding(ContentControl.ContentTemplateProperty, itemTemplateBinding);

            DashboardRoot.Children.Add(contentControl);
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
    }
}
