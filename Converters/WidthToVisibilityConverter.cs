using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TP203.Converters;

public class WidthToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double width && parameter is string thresholdStr && double.TryParse(thresholdStr, out double threshold))
        {
            return width >= threshold;
        }
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
