using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DragNDropTask
{
    public class DragStartedEventArgs : RoutedEventArgs
    {
        public DragStartedEventArgs() : base(DragStartElementBehavior.DragStartedRoutedEvent)
        {

        }

        public Image? DraggingImage { get; set; }
    }
}
