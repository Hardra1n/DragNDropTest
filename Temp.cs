using System.Windows.Documents;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

public class Temp
{
    //private void StartDragInProcAdorner(MouseEventArgs e)
    //{
    //    // Let's define our DragScope .. In this case it is every thing inside our main window ..
    //    var DragScope = Application.Current.MainWindow.Content as FrameworkElement;
    //    System.Diagnostics.Debug.Assert(DragScope != null);
    //    // We enable Drag & Drop in our scope ...  We are not implementing Drop, so it is OK, but this allows us to get DragOver
    //    bool previousDrop = DragScope.AllowDrop;
    //    DragScope.AllowDrop = true;
    //    // Let's wire our usual events..
    //    // GiveFeedback just tells it to use no standard cursors..
    //    GiveFeedbackEventHandler feedbackhandler = new GiveFeedbackEventHandler(DragSource_GiveFeedback);
    //    this.DragSource.GiveFeedback += feedbackhandler;
    //    // The DragOver event ...
    //    DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
    //    DragScope.PreviewDragOver += draghandler;
    //    // Drag Leave is optional, but write up explains why I like it ..
    //    DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
    //    DragScope.DragLeave += dragleavehandler;
    //    // QueryContinue Drag goes with drag leave...
    //    QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
    //    DragScope.QueryContinueDrag += queryhandler;
    //    //Here we create our adorner..
    //    _adorner = new DragAdorner(DragScope, (UIElement)this.dragElement, true, 0.5);
    //    _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
    //    _layer.Add(_adorner); IsDragging = true;
    //    _dragHasLeftScope = false;
    //    //Finally lets drag drop
    //    DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "abcd");
    //    DragDropEffects de = DragDrop.DoDragDrop(this.DragSource, data, DragDropEffects.Move);
    //    // Clean up our mess :)             DragScope.AllowDrop = previousDrop;
    //    AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
    //    _adorner = null;
    //    DragSource.GiveFeedback -= feedbackhandler;
    //    DragScope.DragLeave -= dragleavehandler;
    //    DragScope.QueryContinueDrag -= queryhandler;
    //    DragScope.PreviewDragOver -= draghandler;
    //    IsDragging = false;
    //}
}