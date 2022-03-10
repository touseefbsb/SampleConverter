using System;
using Windows.UI.Xaml.Data;

namespace Shapr3D.Converter.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => ((TimeSpan?)value)?.ToString(@"hh\:mm\:ss") ?? string.Empty;
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
    }
}
