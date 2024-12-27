using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DragNDropTask
{
    internal static class Extensions
    {
        public static ObservableCollection<LayoutSetting> CreateDefaultLayouts()
        {
            ObservableCollection<LayoutSetting> layouts = [];
            LayoutSetting layoutSetting1 = new(1, 1);
            layoutSetting1.AddElement(0, 0, 0, 0);
            layouts.Add(layoutSetting1);
            layoutSetting1.Name = "1";

            LayoutSetting layoutSetting2 = new(1, 2);
            layoutSetting2.AddElement(0, 0, 0, 0);
            layoutSetting2.AddElement(0, 1, 0, 1);
            layouts.Add(layoutSetting2);
            layoutSetting2.Name = "1-1";

            LayoutSetting layoutSetting3 = new(2, 1);
            layoutSetting3.AddElement(0, 0, 0, 0);
            layoutSetting3.AddElement(1, 0, 1, 0);
            layouts.Add(layoutSetting3);

            layoutSetting3.Name = "1-1";

            LayoutSetting layoutSetting4 = new(2, 2);
            layoutSetting4.AddElement(0, 0, 1, 0);
            layoutSetting4.AddElement(0, 1, 0, 1);
            layoutSetting4.AddElement(1, 1, 1, 1);
            layouts.Add(layoutSetting4);
            layoutSetting4.Name = "1-2";

            LayoutSetting layoutSetting5 = new(2, 2);
            layoutSetting5.AddElement(0, 0, 0, 0);
            layoutSetting5.AddElement(0, 1, 0, 1);
            layoutSetting5.AddElement(1, 0, 1, 1);
            layouts.Add(layoutSetting5);
            layoutSetting5.Name = "2-1";

            LayoutSetting layoutSetting6 = new(2, 2);
            layoutSetting6.AddElement(0, 0, 0, 0);
            layoutSetting6.AddElement(0, 1, 0, 1);
            layoutSetting6.AddElement(1, 0, 1, 0);
            layoutSetting6.AddElement(1, 1, 1, 1);
            layouts.Add(layoutSetting6);
            layoutSetting6.Name = "2-2";

            LayoutSetting layoutSetting7 = new(2, 3);
            layoutSetting7.AddElement(0, 0, 1, 0);
            layoutSetting7.AddElement(0, 1, 0, 1);
            layoutSetting7.AddElement(1, 1, 1, 1);
            layoutSetting7.AddElement(0, 2, 0, 2);
            layoutSetting7.AddElement(1, 2, 1, 2);
            layouts.Add(layoutSetting7);
            layoutSetting7.Name = "1-2-2";

            LayoutSetting layoutSetting8 = new(2, 3);
            layoutSetting8.AddElement(0, 0, 0, 0);
            layoutSetting8.AddElement(0, 1, 0, 1);
            layoutSetting8.AddElement(1, 0, 1, 0);
            layoutSetting8.AddElement(1, 1, 1, 1);
            layoutSetting8.AddElement(0, 2, 0, 2);
            layoutSetting8.AddElement(1, 2, 1, 2);
            layouts.Add(layoutSetting8);
            layoutSetting8.Name = "2-2-2";

            LayoutSetting layoutSetting9 = new(3, 3);

            layoutSetting9.AddElement(0, 0, 0, 0);
            layoutSetting9.AddElement(0, 1, 0, 1);
            layoutSetting9.AddElement(1, 0, 1, 0);
            layoutSetting9.AddElement(1, 1, 1, 1);
            layoutSetting9.AddElement(0, 2, 0, 2);
            layoutSetting9.AddElement(1, 2, 1, 2);
            layoutSetting9.AddElement(2, 0, 2, 0);
            layoutSetting9.AddElement(2, 1, 2, 1);
            layoutSetting9.AddElement(2, 2, 2, 2);
            layouts.Add(layoutSetting9);
            layoutSetting9.Name = "3-3-3";

            return layouts;
        }

        public static ObservableCollection<WidgetViewModel>? CreateDefaultItemsSource()
        {
            ObservableCollection<WidgetViewModel> widgets = new();
            const int NUMBER_OF_OBJECTS = 5;

            for (int i = 0; i < NUMBER_OF_OBJECTS; i++)
            {
                Random rand = new();
                widgets.Add(new WidgetViewModel()
                {
                    PosIndex = i, 
                    Content = new Rectangle()
                    {
                        Fill = new SolidColorBrush(Color.FromRgb(
                            (byte)rand.Next(256), 
                            (byte)rand.Next(256), 
                            (byte)rand.Next(256)))
                    }

                });
            }

            return widgets;
        }
    }
}
