using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DragNDropTask
{
    internal class PositionIndexPairConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values.Length == 2 &&
                int.TryParse(values[0].ToString(), out var value1) &&
                int.TryParse(values[1].ToString(), out var value2))
            {
                return new PositionIndexesPair(value1, value2);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return [Binding.DoNothing];
        }
    }
}
