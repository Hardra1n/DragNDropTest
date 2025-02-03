using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DragNDropTask
{
    public class StartDragNDropBehaviour : Behavior<UIElement>
    {
        public static readonly DependencyProperty StartDragNDropCommandProperty = DependencyProperty.Register(nameof(StartDragNDropCommand), typeof(ICommand), typeof(StartDragNDropBehaviour), null);
        public static readonly DependencyProperty PositionIndexProperty = DependencyProperty.Register(nameof(PositionIndex), typeof(int), typeof(StartDragNDropBehaviour), null);



        public ICommand StartDragNDropCommand 
        {
            get => (ICommand)GetValue(StartDragNDropCommandProperty);
            set => SetValue(StartDragNDropCommandProperty, value); 
        }
        
        public int PositionIndex
        {
            get => (int)GetValue(PositionIndexProperty);
            set => SetValue(PositionIndexProperty, value);
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
            if (e.LeftButton == MouseButtonState.Pressed && StartDragNDropCommand.CanExecute(PositionIndex))
            {
                StartDragNDropCommand.Execute(PositionIndex);
            }
        }
    }
}
