using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ICGFilter.Domain.Services;

public class EnumEqualsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(parameter) ?? false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is bool b && b) ? parameter : BindingOperations.DoNothing;
    }
}