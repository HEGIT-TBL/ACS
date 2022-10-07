using ACS.Helpers;
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace ACS.Converters
{
    internal class EnumToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return bruh(value as IEnumerable);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        private IEnumerable bruh(IEnumerable value)
        {
            foreach (var item in value)
                yield return ((Enum)item).GetDescription();
        }
    }
}
