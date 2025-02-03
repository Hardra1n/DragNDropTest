using DragNDropTask.Dashboards;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace DragNDropTask
{
    public class DragNDropElementBehaviour : Behavior<UIElement>
    {
        public static readonly DependencyProperty SwapElementsCommandProperty = 
            DependencyProperty.Register(nameof(SwapElementsCommand), typeof(ICommand), typeof(DragNDropElementBehaviour), null);

        public static readonly DependencyProperty ElementToDropProperty = 
            DependencyProperty.Register(nameof(ElementToDrop), typeof(UIElement), typeof(DragNDropElementBehaviour), null);

        public UIElement ElementToDrop
        {
            get => (UIElement)GetValue(ElementToDropProperty);
            set => SetValue(ElementToDropProperty, value);
        }

        public ICommand SwapElementsCommand
        {
            get => (ICommand)GetValue(SwapElementsCommandProperty);
            set => SetValue(SwapElementsCommandProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragEnter += OnEnter;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.DragLeave += OnDragLeave;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DragEnter -= OnEnter;
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.DragLeave -= OnDragLeave;
        }


        public void ApplyDragEffect(UIElement element)
        {
            element.Effect = new DropShadowEffect();
        }

        private void RefrainDragEffect(UIElement element)
        {
            element.Effect = null;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (sender is not UIElement targetElement)
            {
                return;
            }
            var elem = e.Data.GetData(typeof(UIElement));
            if (e.Data.GetData(typeof(UIElement)) is not UIElement sourceElem)
            {
                return;
            }

            var parameters = new SwapElementsCommandParameters(ElementToDrop, sourceElem);
            if (SwapElementsCommand.CanExecute(parameters))
            {
                SwapElementsCommand.Execute(parameters);
            }

            RefrainDragEffect(targetElement);
        }

        private void OnEnter(object sender, DragEventArgs e)
        {
            if (sender is not UIElement element)
            {
                return;
            }

            ApplyDragEffect(element);
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            if (sender is not UIElement element)
            {
                return;
            }

            RefrainDragEffect(element);
        }

        public void StartDragDrop()
        {
            DataObject dataObj = new();
            dataObj.SetData(typeof(UIElement), AssociatedObject);
            DragDrop.DoDragDrop(AssociatedObject, dataObj, DragDropEffects.Move);
        }
    }
}
