using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DragNDropTask
{
    public class MyAdorner : Adorner
    {
        public Point LastCalculatedMousePosition { get; private set; }

        private bool IsDragging { get; set; }

        public Image DraggingImage;

        private Border border;

        public MyAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = true;
            adornedElement.AllowDrop = true;
            DragStartElementBehavior.AddDragStartedHandler(adornedElement, OnDragStarted);
            DragStartElementBehavior.AddDragEndedHandler(adornedElement, OnDragEnded);
            adornedElement.PreviewDragOver += HandleDragOver;
            adornedElement.QueryContinueDrag += HandleQueryContinueDrag;


            border = new Border
            {
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(2)
            };

            border.DragEnter += Border_DragEnter;

            AddVisualChild(border);
        }

        private void OnDragEnded(object sender, RoutedEventArgs e)
        {
            IsDragging = false;
            InvalidateVisual();
        }

        private void Border_DragEnter(object sender, DragEventArgs e)
        {
            IsDragging = false;
        }

        private void HandleQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (IsDragging)
            {
                return;
            }

            e.Action = DragAction.Cancel;
            e.Handled = true;
            InvalidateVisual();
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => border;

        protected override Size MeasureOverride(Size constraint)
        {
            border.Measure(AdornedElement.RenderSize);
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            border.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs ea)
        {
            if (ea.DraggingImage == null)
            {
                return;
            }

            DraggingImage = ea.DraggingImage;
            IsDragging = true;
        }

        private void HandleDragOver(object sender, DragEventArgs e)
        {
            LastCalculatedMousePosition = e.GetPosition(AdornedElement);
            InvalidateVisual();
            e.Handled = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (IsDragging)
            {
                RenderDragImage(drawingContext);
            }
            base.OnRender(drawingContext);
        }

        private void RenderDragImage(DrawingContext drawingContext)
        {
            DraggingImage.IsHitTestVisible = false;
            var imagePosition = new Point(LastCalculatedMousePosition.X + 15, LastCalculatedMousePosition.Y + 15);
            drawingContext.DrawImage(DraggingImage.Source, new Rect(imagePosition, new Size(100, 100)));
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var position = hitTestParameters.HitPoint;

            if (border.IsVisible && border.InputHitTest(position) != null)
            {
                return new PointHitTestResult(border, position);
            }
            return null;
        }
    }
}
