using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace DragNDropTask.Dashboards;

internal class DragDropHelper
{
    private DependencyObject _dragSource;
    public DragDropHelper(DependencyObject dragSource)
    {
        _dragSource = dragSource;
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


    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var data = new DataObject(typeof(UIElement), element);
            DragDrop.DoDragDrop(_dragSource, data, DragDropEffects.Move);
        }
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        var sourceElem = e.Data.GetData(typeof(UIElement)) as UIElement;
        if (sourceElem == null)
        {
            return;
        }

        var positionDest = WidgetPosition.GetWidgetPositionByElement(element);
        var positionSource = WidgetPosition.GetWidgetPositionByElement(sourceElem);


        sourceElem.SetWidgetPositionOnDashboard(positionDest);
        element.SetWidgetPositionOnDashboard(positionSource);

        element.Effect = null;
    }

    private void OnEnter(object sender, DragEventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        element.Effect = new DropShadowEffect();
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        element.Effect = null;
    }
}