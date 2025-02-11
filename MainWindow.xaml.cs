using System.Windows;
using System.Windows.Documents;

namespace DragNDropTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //var elementToAdorn = FindName("DashboardControlElement") as UIElement;
            //if (elementToAdorn == null)
            //{
            //    return;
            //}

            //var adorer = AdornerLayer.GetAdornerLayer(elementToAdorn);

            //adorer?.Add(new DragDropAdorner(elementToAdorn));
        }
    }
}