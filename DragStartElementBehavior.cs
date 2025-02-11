using System;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors;
using NLog;

namespace DragNDropTask;

public class DragStartElementBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty PositionIndexProperty = DependencyProperty.Register(
                                                    nameof(PositionIndex), typeof(int), typeof(DragStartElementBehavior), new PropertyMetadata(default(int)));

    public static readonly DependencyProperty IsDragEnabledProperty = DependencyProperty.Register(
                                                    nameof(IsDragEnabled), typeof(bool), typeof(DragStartElementBehavior), new PropertyMetadata(default(bool)));

    public static readonly RoutedEvent DragStartedRoutedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragStartElementBehavior));

    public static readonly RoutedEvent DragEndedRoutedEvent = EventManager.RegisterRoutedEvent("DragEnded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragStartElementBehavior));


    public static void AddDragEndedHandler(DependencyObject element, RoutedEventHandler handler)
    {
        if (element is not UIElement uiElement)
        {
            return;
        }

        uiElement.AddHandler(DragEndedRoutedEvent, handler);
    }

    public static void RemoveDragEndedHandler(DependencyObject element, RoutedEventHandler handler)
    {
        if (element is not UIElement uiElement)
        {
            return;
        }

        uiElement.RemoveHandler(DragEndedRoutedEvent, handler);
    }

    public static void AddDragStartedHandler(DependencyObject element, RoutedEventHandler handler)
    {
        if (element is not UIElement uiElement)
        {
            return;
        }

        uiElement.AddHandler(DragStartedRoutedEvent, handler);
    }

    public static void RemoveDragStartedHandler(DependencyObject element, RoutedEventHandler handler)
    {
        if (element is not UIElement uiElement)
        {
            return;
        }

        uiElement.RemoveHandler(DragStartedRoutedEvent, handler);
    }

    public bool IsDragEnabled
    {
        get => (bool) GetValue(IsDragEnabledProperty);
        set => SetValue(IsDragEnabledProperty, value);
    }

    public int PositionIndex
    {
        get => (int) GetValue(PositionIndexProperty);
        set => SetValue(PositionIndexProperty, value);
    }

    public bool IsLeftButtonPressedOnElement { get; set; }

    protected override void OnAttached()
    {
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    private void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
    {
        IsLeftButtonPressedOnElement = true;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            IsLeftButtonPressedOnElement = false;
            return;
        }

        if (!IsLeftButtonPressedOnElement || !IsDragEnabled)
        {
            return;
        }

        DataObject dataObject = new();
        dataObject.SetData(typeof(int), PositionIndex);
        dataObject.SetData(typeof(Type), typeof(DragStartElementBehavior));
        var eventArgs = new DragStartedEventArgs();
        AssociatedObject.RaiseEvent(eventArgs);

        DragDrop.DoDragDrop(AssociatedObject, dataObject, DragDropEffects.Move);

        AssociatedObject.RaiseEvent(new RoutedEventArgs(DragEndedRoutedEvent));
        IsLeftButtonPressedOnElement = false;
    }
}
