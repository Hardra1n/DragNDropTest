using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors;
using NLog;

namespace DragNDropTask;

public class DropElementBehaviour : Behavior<FrameworkElement>
{
    private const bool DefaultAllowDropValue = false;

    public static readonly DependencyProperty PositionIndexProperty =
        DependencyProperty.Register(nameof(PositionIndex), typeof(int), typeof(DropElementBehaviour),
            new PropertyMetadata(default(int)));

    public static readonly DependencyProperty SwapWidgetsCommandProperty =
        DependencyProperty.Register(nameof(SwapWidgetsCommand), typeof(ICommand), typeof(DropElementBehaviour), null);

    public static readonly DependencyProperty AllowDropProperty = DependencyProperty.Register(
        nameof(AllowDrop), typeof(bool), typeof(DropElementBehaviour),
        new PropertyMetadata(DefaultAllowDropValue, AllowDropPropertyChanged));

    public static readonly DependencyProperty MyStackPanelProperty = DependencyProperty.Register(
        nameof(MyStackPanel), typeof(StackPanel), typeof(DropElementBehaviour),
        new PropertyMetadata(default(StackPanel)));

    public static readonly DependencyProperty MyTextBlockProperty = DependencyProperty.Register(
        nameof(MyTextBlock), typeof(TextBlock), typeof(DropElementBehaviour), new PropertyMetadata(default(TextBlock)));

    //public static readonly RoutedEvent DragEndedRoutedEvent = EventManager.RegisterRoutedEvent("DragEnded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropElementBehaviour));

    //public static void AddDragEndedHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
    //{
    //    if (dependencyObject is not UIElement element)
    //    {
    //        return;
    //    }

    //    element.AddHandler(DragEndedRoutedEvent, handler);
    //}

    //public static void RemoveDragEndedHandler(DependencyObject dependencyObject, RoutedEventHandler handler)
    //{
    //    if (dependencyObject is not UIElement element)
    //    {
    //        return;
    //    }

    //    element.RemoveHandler(DragEndedRoutedEvent, handler);
    //}



    public TextBlock MyTextBlock
    {
        get { return (TextBlock)GetValue(MyTextBlockProperty); }
        set { SetValue(MyTextBlockProperty, value); }
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public StackPanel MyStackPanel
    {
        get { return (StackPanel)GetValue(MyStackPanelProperty); }
        set { SetValue(MyStackPanelProperty, value); }
    }

    public bool AllowDrop
    {
        get => (bool)GetValue(AllowDropProperty);
        set => SetValue(AllowDropProperty, value);
    }

    public int PositionIndex
    {
        get => (int)GetValue(PositionIndexProperty);
        set => SetValue(PositionIndexProperty, value);
    }

    public ICommand SwapWidgetsCommand
    {
        get => (ICommand)GetValue(SwapWidgetsCommandProperty);
        set => SetValue(SwapWidgetsCommandProperty, value);
    }

    private Effect SavedAssociatedObjectEffect { get; set; }

    private Window ParentWindow { get; set; }

    private Image DraggingImage { get; set; }

    private Point PreviousDraggingMousePosition { get; set; }



    private static void AllowDropPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DropElementBehaviour behavior || e.NewValue is not bool isAllowed ||
            behavior.AssociatedObject == null)
        {
            return;
        }
    }

    private void AllowDropForChildren(DependencyObject obj)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is UIElement element)
            {
                element.AllowDrop = true;
            }

            if (child is ContentControl contentControl && contentControl.Content is UIElement innerElement)
            {
                innerElement.AllowDrop = true;
                AllowDropForChildren(innerElement);
            }

            AllowDropForChildren(child);
        }

        if (obj is UIElement elem)
        {
            elem.AllowDrop = true;
        }
    }


    protected override void OnAttached()
    {
        AllowDrop = true;
        ParentWindow = Extensions.FindAncestor<Window>(AssociatedObject);
        AllowDropForChildren(AssociatedObject);
        SavedAssociatedObjectEffect = AssociatedObject.Effect;


        AssociatedObject.DragEnter += OnDragEnter;
        AssociatedObject.DragLeave += OnDragLeave;
        AssociatedObject.Drop += OnDrop;
        AssociatedObject.DragOver += OnDragOver;
        DragStartElementBehavior.AddDragStartedHandler(AssociatedObject, OnDragStarted);
    }

    private void OnDragStarted(object sender, DragStartedEventArgs eventArgs)
    {
        eventArgs.DraggingImage = CreateImage(100, 100);
    }

    protected override void OnDetaching()
    {
        AllowDrop = false;
        AssociatedObject.DragEnter += OnDragEnter;
        AssociatedObject.DragLeave += OnDragLeave;
        AssociatedObject.Drop += OnDrop;
        AssociatedObject.DragOver += OnDragOver;
        DragStartElementBehavior.RemoveDragStartedHandler(AssociatedObject, OnDragStarted);
    }

    private Image CreateImage(double width, double height)
    {
        var renderTargetBitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);

        var visual = new DrawingVisual();

        using (var context = visual.RenderOpen())
        {
            context.DrawRectangle(new VisualBrush(AssociatedObject), null, new Rect(new Size(width, height)));
        }

        renderTargetBitmap.Render(visual);

        Image image = new()
        {
            Source = renderTargetBitmap
        };
        return image;
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        if (!IsDragDropOperationAllowed(e, out _))
        {
            return;
        }

        e.Effects = DragDropEffects.Move;
        //e.Handled = true;
    }

    private void OnDragEnter(object sender, DragEventArgs e)
    {
        if (!IsDragDropOperationAllowed(e, out _))
        {
            return;
        }

        e.Effects = DragDropEffects.Move;
        ApplyDragEffect();
        e.Handled = true;
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        if (!IsDragDropOperationAllowed(e, out _))
        {
            return;
        }

        RefrainDragEffect();
        e.Handled = true;
    }


    private void ApplyDragEffect()
    {
        AssociatedObject.Effect = new DropShadowEffect();
    }

    private void RefrainDragEffect()
    {
        AssociatedObject.Effect = SavedAssociatedObjectEffect;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (!IsDragDropOperationAllowed(e, out var sourceIndex))
        {
            return;
        }

        List<int> parameters = [sourceIndex, PositionIndex];

        if (SwapWidgetsCommand.CanExecute(parameters))
        {
            SwapWidgetsCommand.Execute(parameters);
        }

        RefrainDragEffect();
    }

    private bool IsDragDropOperationAllowed(DragEventArgs eventArgs, out int positionIndex)
    {
        positionIndex = -1;
        if (eventArgs.Data.GetData(typeof(int)) is not int index ||
            eventArgs.Data.GetData(typeof(string)) is not string targetBehavior ||
            !targetBehavior.Equals(nameof(DropElementBehaviour)))
        {
            return false;
        }

        if (index == PositionIndex || !AllowDrop)
        {
            eventArgs.Effects = DragDropEffects.None;
            //eventArgs.Handled = true;
            return false;
        }

        positionIndex = index;
        eventArgs.Effects = DragDropEffects.Move;
        return true;
    }
}