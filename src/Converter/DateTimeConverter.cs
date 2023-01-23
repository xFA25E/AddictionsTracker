using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AddictionsTracker.Converter;

public class DateTimeConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        // For add this is simple. just return the sum of the value and the parameter.
        // You may want to validate value and parameter in a real world App
        // return (double?)value + (double?)parameter;
        return ((DateTime?)value).Value.ToString((string?)parameter);
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        // If we want to convert back, we need to subtract instead of add.
        // return (double?)value - (double?)parameter;
        return BindingOperations.DoNothing;
    }
}
