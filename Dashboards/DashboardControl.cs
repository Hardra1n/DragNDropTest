using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DragNDropTask.Dashboards
{
    [TemplatePart(Name = "DashboardRoot", Type =  typeof(Grid))]
    public class DashboardControl : Control
    {
        private ItemContainerGenerator? _itemContainerGenerator = null;

        public static readonly DependencyProperty LayoutSettingProperty
            = DependencyProperty.Register(nameof(LayoutSetting), typeof(LayoutSetting), typeof(DashboardControl),
                new PropertyMetadata(null, OnLayoutSettingChanged));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource), typeof(IEnumerable), typeof(DashboardControl), new PropertyMetadata(default(IEnumerable)));

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate), typeof(DataTemplate), typeof(DashboardControl), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private Grid DashboardRoot { get; set; }

        public LayoutSetting LayoutSetting
        {
            get => (LayoutSetting)GetValue(LayoutSettingProperty);
            set => SetValue(LayoutSettingProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }



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
            if (GetTemplateChild("DashboardRoot") is not Grid dashboardRoot)
            {
                throw new ArgumentException("Unable to find grid with 'DashboardRoot' name");
            }


            DashboardRoot = dashboardRoot;
        }

        private void HandleLayoutSettingsChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
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

                AddElementToDashboard(positions[i], item);
            }
        }

        private void ClearCurrentLayoutInformation()
        {
            DashboardRoot.RowDefinitions.Clear();
            DashboardRoot.ColumnDefinitions.Clear();
            foreach (var child in DashboardRoot.Children)
            {
                if (child is UIElement element)
                {
                    element.MouseMove -= MouseMoveHanlder;
                    element.DragEnter -= DragEnterTempMethod;
                    element.Drop -= DropTempMethod;
                    element.DragLeave -= DragLeaveTempMethod;
                }
            }
            DashboardRoot.Children.RemoveRange(0, DashboardRoot.Children.Count);
        }

        private void AddElementToDashboard(WidgetPosition position, object item)
        {
            if (item is not WidgetViewModel widgetVM)
                return;
            if (widgetVM.Content is not UIElement element)
                return;
            ContentControl contentControl = new ContentControl();

            contentControl.AllowDrop = true;
            contentControl.MouseMove += MouseMoveHanlder;
            contentControl.DragEnter += DragEnterTempMethod;
            contentControl.Drop += DropTempMethod;
            contentControl.DragLeave += DragLeaveTempMethod;
            position.SetWidgetPosition(contentControl);
            contentControl.Content = element;

            Binding itemTemplateBinding = new Binding(nameof(ItemTemplate))
            {
                Source = this
            };
            contentControl.SetBinding(ContentControl.ContentTemplateProperty, itemTemplateBinding);

            DashboardRoot.Children.Add(contentControl);
        }


        private void MouseMoveHanlder(object sender, MouseEventArgs e)
        {
            if (sender is not UIElement element)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var data = new DataObject(typeof(UIElement), element);
                DragDrop.DoDragDrop(DashboardRoot, data, DragDropEffects.Move);
            }
        }

        private void DropTempMethod(object sender, DragEventArgs e)
        {
            if (sender is not UIElement element)
            {
                return;
            }

            var sourceElem = e.Data.GetData(typeof(UIElement)) as UIElement;
            if (sourceElem == null)
            {
                return;
            }

            var positionDest = WidgetPosition.GetWidgetPositionByElement(element);
            var positionSource = WidgetPosition.GetWidgetPositionByElement(sourceElem);
            

            positionDest.SetWidgetPosition(sourceElem);
            positionSource.SetWidgetPosition(element);

            element.Effect = null;
        }

        private void DragEnterTempMethod(object sender, DragEventArgs e)
        {
            if (sender is not UIElement element)
            {
                return;
            }

            element.Effect = new DropShadowEffect();
        }
        private void DragLeaveTempMethod(object sender, DragEventArgs e)
        {
            if (sender is not UIElement element)
            {
                return;
            }

            element.Effect = null;
        }

        private WidgetPosition[] GetPositionsByLayoutSettings()
        {
            Dictionary<int, WidgetPosition> positions = new Dictionary<int, WidgetPosition>();
            for (int i = 0; i < LayoutSetting.RowCount; i++)
            {
                for (int j = 0; j < LayoutSetting.ColumnCount; j++)
                {
                    int currentElemIndex = LayoutSetting.ElementsScheme[i, j];
                    if (positions.ContainsKey(currentElemIndex) || currentElemIndex == 0)
                    {
                        continue;
                    }
                    else
                    {
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
            }

            return positions.Values.ToArray();
        }

        private sealed class WidgetPosition
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

            public void SetWidgetPosition(UIElement element)
            {
                element.SetValue(Grid.RowProperty, Row);
                element.SetValue(Grid.RowSpanProperty, RowSpan);
                element.SetValue(Grid.ColumnProperty, Column);
                element.SetValue(Grid.ColumnSpanProperty, ColumnSpan);
            }
        }
    }
}
