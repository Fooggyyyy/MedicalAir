using System;
using System.Globalization;
using System.Windows.Data;

namespace MedicalAir.Helper.Converters
{
    public class BoolToTestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool mustBeTrue)
            {
                return mustBeTrue ? "Тест" : "Числовая";
            }
            return "Числовая";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
