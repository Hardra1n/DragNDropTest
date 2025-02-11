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
using System.Windows.Shapes;
using DragNDropTask.Dashboards;
using NLog.LayoutRenderers;

namespace DragNDropTask
{
    public class MyAdorner : Adorner
    {
        private const int ImageOffsetX = 15;

        private const int ImageOffsetY = 15;

        private readonly Size ImageSize = new Size(100, 100);

        private readonly Border _windowSizeBorder;

        private Point LastCalculatedMousePosition { get; set; }

        private bool IsDragging { get; set; }

        public Image DraggingImage;


        public MyAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = true;
            adornedElement.AllowDrop = true;
            DragStartElementBehavior.AddDragStartedHandler(adornedElement, OnDragStarted);
            DragStartElementBehavior.AddDragEndedHandler(adornedElement, OnDragEnded);
            adornedElement.DragOver += HandleDragOver;
            adornedElement.QueryContinueDrag += HandleQueryContinueDrag;

            _windowSizeBorder = new Border()
            {
                Background = Brushes.Transparent,
                Visibility = Visibility.Hidden
            };

            DraggingImage = new Image()
            {
                RenderTransform = new TranslateTransform(),
                Visibility = Visibility.Hidden
            };
        }

        private void OnDragEnded(object sender, RoutedEventArgs e)
        {
            IsDragging = false;
            InvalidateVisual();
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

        protected override int VisualChildrenCount => 2;

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return _windowSizeBorder;
            }
            else
            {
                return DraggingImage;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //DraggingImage.Arrange(new Rect(ImageSize));
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var window = Extensions.FindAncestor<Window>(AdornedElement);
            var width = window.RenderSize.Width;
            var height = window.RenderSize.Height;
            Rect fullWindowSizeRect = new Rect(new Point(-width, -height), new Size(width * 2, height * 2));
            _windowSizeBorder.Arrange(fullWindowSizeRect);

            DraggingImage.Arrange(new Rect(new Point(LastCalculatedMousePosition.X + ImageOffsetX, LastCalculatedMousePosition.Y + ImageOffsetY), ImageSize));
            return finalSize;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs ea)
        {
            if (ea.DraggingImage == null)
            {
                return;
            }

            if (ea.DraggingImage != null)
            {
                DraggingImage.Source = ea.DraggingImage.Source;
                DraggingImage.Visibility = Visibility.Visible;
                IsDragging = true;
                InvalidateVisual();
            }
        }

        private void HandleDragOver(object sender, DragEventArgs e)
        {
            LastCalculatedMousePosition = e.GetPosition(AdornedElement);
            InvalidateVisual();

            e.Handled = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {

            //if (DraggingImage.RenderTransform is TranslateTransform translateTransform)
            //{
            //    translateTransform.X = LastCalculatedMousePosition.X + ImageOffsetX;
            //    translateTransform.Y = LastCalculatedMousePosition.Y + ImageOffsetY;
            //}
            //if (IsDragging)
            //{
            //    RenderDragImage(drawingContext);
            //}

            base.OnRender(drawingContext);
        }

        //private void RenderDragImage(DrawingContext drawingContext)
        //{
        //    DraggingImage.IsHitTestVisible = false;

        //    var imagePosition = new Point(LastCalculatedMousePosition.X + 15, LastCalculatedMousePosition.Y + 15);
        //    drawingContext.DrawImage(DraggingImage.Source, new Rect(imagePosition, new Size(100, 100)));
        //}

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var position = hitTestParameters.HitPoint;

            var adornedElementHitTest = AdornedElement.InputHitTest(position);

            if (adornedElementHitTest == null)
            {
                IsDragging = false;
            }

            return null;
        }
    }
}
