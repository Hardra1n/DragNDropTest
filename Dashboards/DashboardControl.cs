using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace DragNDropTask.Dashboards
{
    [TemplatePart(Name = "DashboardRoot", Type =  typeof(Grid))]
    internal class DashboardControl : Control
    {

        public static readonly DependencyProperty LayoutSettingProperty
            = DependencyProperty.Register(nameof(LayoutSetting2), typeof(LayoutSetting), typeof(DashboardControl),
                new PropertyMetadata(null, OnLayoutSettingChanged));

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource), typeof(IEnumerable), typeof(DashboardControl), new PropertyMetadata(default(IEnumerable)));



        private Grid DashboardRoot { get; set; }

        public LayoutSetting LayoutSetting2
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
            if (item is not UIElement element)
                return;
            element.AllowDrop = true;
            element.MouseMove += MouseMoveHanlder;
            element.DragEnter += DragEnterTempMethod;
            element.Drop += DropTempMethod;
            element.DragLeave += DragLeaveTempMethod;
            position.SetWidgetPosition(element);
            DashboardRoot.Children.Add(element);
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
            for (int i = 0; i < LayoutSetting2.RowCount; i++)
            {
                for (int j = 0; j < LayoutSetting2.ColumnCount; j++)
                {
                    int currentElemIndex = LayoutSetting2.ElementsScheme[i, j];
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
                        while (i + k < LayoutSetting2.RowCount)
                        {
                            if (LayoutSetting2.ElementsScheme[i + k, j] != currentElemIndex)
                            {
                                break;
                            }

                            k++;
                        }

                        widgetPosition.RowSpan = k;

                        k = 1;
                        while (j + k < LayoutSetting2.ColumnCount)
                        {
                            if (LayoutSetting2.ElementsScheme[i, j + k] != currentElemIndex)
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

        private sealed class DragNDropData
        {
            public WidgetPosition Position { get; init; }
            public UIElement Element { get; init; }
        }
    }
}
