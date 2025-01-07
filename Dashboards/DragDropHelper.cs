using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace DragNDropTask.Dashboards;

internal class DragDropHelper
{
    private readonly DependencyObject _dragSource;
    private readonly Action<UIElement, UIElement> _dropEndActionHook;

    public DragDropHelper(DependencyObject dragSource, Action<UIElement, UIElement> dropEndActionHook)
    {
        _dragSource = dragSource;
        _dropEndActionHook = dropEndActionHook;
    }

    public void Register(UIElement element)
    {

        element.AllowDrop = true;
        element.MouseMove += OnMouseMove;
        element.DragEnter += OnEnter;
        element.Drop += OnDrop;
        element.DragLeave += OnDragLeave;
    }

    public void Unregister(UIElement element)
    {
        element.MouseMove -= OnMouseMove;
        element.DragEnter -= OnEnter;
        element.Drop -= OnDrop;
        element.DragLeave -= OnDragLeave;
    }

    public void ApplyDragEffect(UIElement element)
    {
        element.Effect = new DropShadowEffect();
    }

    private void RefrainDragEffect(UIElement element)
    {
        element.Effect = null;
    }


    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var data = new DataObject(typeof(UIElement), element);

        DragDrop.DoDragDrop(_dragSource, data, DragDropEffects.Move);
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (sender is not UIElement targetElement)
        {
            return;
        }

        if (e.Data.GetData(typeof(UIElement)) is not UIElement sourceElem)
        {
            return;
        }

        var positionDest = WidgetPosition.GetWidgetPositionByElement(targetElement);

        var positionSource = WidgetPosition.GetWidgetPositionByElement(sourceElem);


        sourceElem.SetWidgetPositionOnDashboard(positionDest);
        targetElement.SetWidgetPositionOnDashboard(positionSource);

        
        _dropEndActionHook.Invoke(sourceElem, targetElement);

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
}