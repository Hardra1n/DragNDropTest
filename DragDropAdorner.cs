using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    internal class DragDropAdorner : Adorner
    {
        private const int ImageOffsetX = 15;

        private const int ImageOffsetY = 15;

        private readonly Size ImageSize = new Size(100, 100);

        private readonly Border _windowSizeBorder;

        private readonly Border _widgetPanelSizeBorder;

        private readonly TranslateTransform _translateTransform;

        private Point _lastMousePosition { get; set; }

        private bool IsDragging { get; set; }

        private VisualCollection AdornerVisuals;


        public Image DraggingImage;

        public DragDropAdorner(UIElement adornedElement, ImageSource source) : base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(this);
            IsHitTestVisible = true;
            adornedElement.AllowDrop = true;
            DragStartElementBehavior.AddDragEndedHandler(adornedElement, OnDragEnded);
            adornedElement.DragOver += HandleDragOver;
            adornedElement.QueryContinueDrag += HandleQueryContinueDrag;
            adornedElement.DragEnter += HandleDragEnter;
            adornedElement.DragLeave += HandleDragLeave;

            _windowSizeBorder = new Border()
            {
                Background = Brushes.Transparent,
                Visibility = Visibility.Hidden
            };

            _widgetPanelSizeBorder = new Border()
            {
                Background = Brushes.Transparent,
                Visibility = Visibility.Hidden
            };

            _translateTransform = new TranslateTransform();

            DraggingImage = new Image()
            {
                Source = source,
                Width = ImageSize.Width,
                Height = ImageSize.Height,
                IsHitTestVisible = false,
                RenderTransform = _translateTransform
            };

            AdornerVisuals.Add(_widgetPanelSizeBorder);
            AdornerVisuals.Add(_windowSizeBorder);
            AdornerVisuals.Add(DraggingImage);

            CompositionTarget.Rendering += HandleRendering;
            IsDragging = true;
        }

        private void HandleDragLeave(object sender, DragEventArgs e)
        {
            if (!CheckDragEvent(e))
            {
                return;
            }

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (!CheckDragEvent(e))
            {
                return;
            }

            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void OnDragEnded(object sender, RoutedEventArgs e)
        {
            UnsubscribeToEvents();
            IsDragging = false;
            InvalidateVisual();
        }

        private void HandleQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (IsDragging)
            {
                return;
            }

            UnsubscribeToEvents();
            e.Action = DragAction.Cancel;
            e.Handled = true;
            InvalidateVisual();
        }

        private void UnsubscribeToEvents()
        {
            CompositionTarget.Rendering -= HandleRendering;
            DragStartElementBehavior.RemoveDragEndedHandler(AdornedElement, OnDragEnded);
            AdornedElement.DragOver -= HandleDragOver;
            AdornedElement.QueryContinueDrag -= HandleQueryContinueDrag;
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;

        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var window = Extensions.FindAncestor<Window>(AdornedElement);
            var width = window.RenderSize.Width;
            var height = window.RenderSize.Height;
            Rect fullWindowSizeRect = new Rect(new Point(-width, -height), new Size(width * 2, height * 2));
            _windowSizeBorder.Arrange(fullWindowSizeRect);

            _widgetPanelSizeBorder.Arrange(new Rect(new Point(0, 0), AdornedElement.RenderSize));

            DraggingImage.Arrange(new Rect(new Point(0, 0), ImageSize));
            return finalSize;
        }


        // Variant 1.1
        //private void HandleDragOver(object sender, DragEventArgs e)
        //{
        //    var position = e.GetPosition(AdornedElement);
        //    _lastMousePosition = position;

        //    UpdateDraggingImagePosition();

        //    e.Handled = true;
        //}

        // Variant 1.2
        //private void HandleDragOver(object sender, DragEventArgs e)
        //{
        //    var position = e.GetPosition(AdornedElement);
        //    _lastMousePosition = position;

        //    if (!_dragUpdateTimer.IsEnabled)
        //    {
        //        _dragUpdateTimer.Start();
        //    }

        //    e.Handled = true;
        //}

        // Variant 1.3

        private void HandleDragOver(object sender, DragEventArgs e)
        {
            var position = e.GetPosition(AdornedElement);
            _lastMousePosition = position;

            e.Handled = true;
        }

        private void HandleRendering(object sender, EventArgs e)
        {
            UpdateDraggingImagePosition();
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var position = hitTestParameters.HitPoint;

            _widgetPanelSizeBorder.Visibility = Visibility.Visible;
            var hitTestWidgetPanelSizeBorder = _widgetPanelSizeBorder.InputHitTest(position);
            _widgetPanelSizeBorder.Visibility = Visibility.Hidden;

            if (hitTestWidgetPanelSizeBorder == null)
            {
                IsDragging = false;
            }

            return null;
        }

        private bool CheckDragEvent(DragEventArgs eventArgs)
        {
            return eventArgs.Data.GetData(typeof(Type)) is Type type && type == typeof(DragStartElementBehavior);
        }


        // Variant 2.1
        private void UpdateDraggingImagePosition()
        {
            _translateTransform.X = _lastMousePosition.X + ImageOffsetX;
            _translateTransform.Y = _lastMousePosition.Y + ImageOffsetY;
        }


        // Variant 2.2
        //private void UpdateDraggingImagePosition()
        //{
        //    InvalidateVisual();
        //}

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    drawingContext.DrawImage(DraggingImage.Source, new Rect(_lastMousePosition, ImageSize));
        //    base.OnRender(drawingContext);
        //}
    }
}
