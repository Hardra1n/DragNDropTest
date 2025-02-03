using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DragNDropTask
{
    public class SwapElementsCommandParameters
    {
        public SwapElementsCommandParameters(UIElement targetElement, UIElement sourceElement)
        {
            TargetElement = targetElement;
            SourceElement = sourceElement;
        }

        public UIElement TargetElement { get; private set; }

        public UIElement SourceElement { get; private set; }
    }
}
