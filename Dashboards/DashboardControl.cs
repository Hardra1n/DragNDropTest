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
    internal class DashboardControl : ItemsControl
    {

        public static readonly DependencyProperty LayoutSettingProperty
            = DependencyProperty.Register(nameof(LayoutSetting), typeof(LayoutSetting), typeof(DashboardControl),
                new PropertyMetadata(null, OnLayoutSettingChanged));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("DashboardRoot") is not Grid dashboardRoot)
            {
                throw new ArgumentException("Unable to find grid with 'DashboardRoot' name");
            }

            DashboardRoot = dashboardRoot;
        }

        private Grid DashboardRoot { get; set; }

        public LayoutSetting LayoutSetting
        {
            get => (LayoutSetting)GetValue(LayoutSettingProperty);
            set => SetValue(LayoutSettingProperty, value);
        }


        private static void OnLayoutSettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DashboardControl dashboardControl)
            {
                dashboardControl.HandleLayoutSettingsChanged(e);
            }
        }

        private void HandleLayoutSettingsChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            ClearCurrentLayoutInformation();

            var rowCount = LayoutSetting.RowCount;
            var columnCount = LayoutSetting.ColumnCount;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DashboardRoot.RowDefinitions.Add(new RowDefinition());
            }

            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                DashboardRoot.ColumnDefinitions.Add(new ColumnDefinition());
            }

            PopulateDashboard();
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

        private void PopulateDashboard()
        {

            if (Items.Count < 1)
                return;

            
            for (int rowIndex = 0, itemIndex = 0; rowIndex < LayoutSetting.RowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < LayoutSetting.ColumnCount; columnIndex++)
                {
                    if (itemIndex >= Items.Count)
                    {
                        return;
                    }
                    
                    var item = Items[itemIndex];
                    if (item != null)
                    {
                        AddElementToDashboard(rowIndex, columnIndex, item);
                        itemIndex++;
                    }
                    
                }
            }
        }

        private void AddElementToDashboard(int rowIndex, int columnIndex, object item)
        {
            if (item is not UIElement element)
                return;
            element.AllowDrop = true;
            element.MouseMove += MouseMoveHanlder;
            element.DragEnter += DragEnterTempMethod;
            element.Drop += DropTempMethod;
            element.DragLeave += DragLeaveTempMethod;
            Grid.SetRow(element, rowIndex);
            Grid.SetColumn(element, columnIndex);
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

            var rowValueDest = Grid.GetRow(element);
            var columnValueDest = Grid.GetColumn(element);
            var rowValueSource = Grid.GetRow(sourceElem);
            var columnValueSource = Grid.GetColumn(sourceElem);
            if (rowValueDest < 0 || columnValueDest < 0 || rowValueSource < 0 || columnValueSource < 0)
                return;

            Grid.SetRow(element, rowValueSource);
            Grid.SetColumn(element, columnValueSource);
            Grid.SetRow(sourceElem, rowValueDest);
            Grid.SetColumn(sourceElem, columnValueDest);

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
    }
}
