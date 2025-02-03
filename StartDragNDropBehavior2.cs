using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DragNDropTask
{
    public class StartDragNDropBehavior2 : Behavior<UIElement>
    {
        public static readonly DependencyProperty ElementToDragProperty = DependencyProperty.Register(nameof(ElementToDrag), typeof(UIElement), typeof(StartDragNDropBehavior2));

        public UIElement ElementToDrag
        {
            get => (UIElement)GetValue(ElementToDragProperty);
            set => SetValue(ElementToDragProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ElementToDrag != null)
            {
                DataObject dataObj = new();
                dataObj.SetData(typeof(UIElement), ElementToDrag);
                DragDrop.DoDragDrop(ElementToDrag, dataObj, DragDropEffects.Move);
            }
        }
    }
}
