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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DragNDropTask.Dashboards;
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


    private Adorner _dragDropAdorner;

    private Effect SavedAssociatedObjectEffect { get; set; }

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
        AllowDropForChildren(AssociatedObject);
        SavedAssociatedObjectEffect = AssociatedObject.Effect;

        AssociatedObject.DragEnter += OnDragEnter;
        AssociatedObject.DragLeave += OnDragLeave;
        AssociatedObject.Drop += OnDrop;
        AssociatedObject.DragOver += OnDragOver;
        DragStartElementBehavior.AddDragStartedHandler(AssociatedObject, OnDragStarted);
        DragStartElementBehavior.AddDragEndedHandler(AssociatedObject, OnDragEnded);
    }

    protected override void OnDetaching()
    {
        AllowDrop = false;
        AssociatedObject.DragEnter -= OnDragEnter;
        AssociatedObject.DragLeave -= OnDragLeave;
        AssociatedObject.Drop -= OnDrop;
        AssociatedObject.DragOver -= OnDragOver;
        DragStartElementBehavior.RemoveDragStartedHandler(AssociatedObject, OnDragStarted);
    }

    private void OnDragEnded(object sender, RoutedEventArgs e)
    {
        var element = FindElementToAttachAdorner();
        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
        if (adornerLayer == null)
        {
            return;
        }

        adornerLayer.Remove(_dragDropAdorner);
    }

    private void OnDragStarted(object sender, RoutedEventArgs e)
    {
        var element = FindElementToAttachAdorner();
        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
        if (adornerLayer == null)
        {
            return;
        }
        _dragDropAdorner = new DragDropAdorner(element, AssociatedObject);

        adornerLayer.Add(_dragDropAdorner);
    }

    private UIElement FindElementToAttachAdorner()
    {

        var widgetsPanelParent = Extensions.FindAncestor<DashboardControl>(AssociatedObject);
        return widgetsPanelParent;
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        if (!IsDragDropOperationAllowed(e, out _))
        {
            return;
        }

        e.Effects = DragDropEffects.Move;

    }

    private void OnDragEnter(object sender, DragEventArgs e)
    {
        if (!IsDragDropOperationAllowed(e, out _))
        {
            e.Handled = true;
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
            e.Handled = true;
            return;
        }

        RefrainDragEffect();
        e.Effects = DragDropEffects.Move;
        e.Handled = true;
    }


    private void ApplyDragEffect()
    {
        AssociatedObject.Effect = new DropShadowEffect
        {
            BlurRadius = 30,
            ShadowDepth = 8,
            RenderingBias = RenderingBias.Performance,
            Direction = 270
        };
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
        e.Handled = true;
    }

    private bool IsDragDropOperationAllowed(DragEventArgs eventArgs, out int positionIndex)
    {
        positionIndex = -1;
        if (eventArgs.Data.GetData(typeof(int)) is not int index ||
            eventArgs.Data.GetData(typeof(Type)) is not Type targetType ||
            targetType != typeof(DragStartElementBehavior))
        {
            return false;
        }

        if (index == PositionIndex || !AllowDrop)
        {
            eventArgs.Effects = DragDropEffects.Move;
            return false;
        }

        positionIndex = index;
        eventArgs.Effects = DragDropEffects.Move;
        return true;
    }
}