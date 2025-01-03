using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DragNDropTask.Dashboards;

namespace DragNDropTask
{
    public class PositionsSwappedRoutedEventArgs : RoutedEventArgs
    {
        public PositionsSwappedRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
        
        public PositionsSwappedRoutedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source) { }

        public PositionIndexesPair Positions;
    }
}
