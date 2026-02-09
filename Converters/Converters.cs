using ATimeVisualCalculator.Models.TimeFilters;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ATimeVisualCalculator.Converters
{
    /// <summary>
    /// Converts bool IsActive to green/red brush for timeline cells
    /// </summary>
    public class BoolToCellBrushConverter : IValueConverter
    {
        // Modern green and red
        private static readonly SolidColorBrush GreenBrush = new(Color.FromRgb(34, 197, 94));   // #22C55E
        private static readonly SolidColorBrush RedBrush = new(Color.FromRgb(239, 68, 68));      // #EF4444

        static BoolToCellBrushConverter()
        {
            GreenBrush.Freeze();
            RedBrush.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true ? GreenBrush : RedBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Converts bool to Visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter is string s && s == "invert";
            bool val;
            if (value is bool b)
                val = b;
            else
                val = value != null; // non-null object → true (visible)
            if (invert) val = !val;
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Converts sidebar open state to grid column width
    /// </summary>
    public class BoolToSidebarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true ? new GridLength(280) : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Converts TimeFilterType enum to display string
    /// </summary>
    public class FilterTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeFilterType ft)
            {
                return ft switch
                {
                    TimeFilterType.And => "∩ Intersect",
                    TimeFilterType.Or => "∪ Union",
                    TimeFilterType.Xor => "⊕ XOR",
                    _ => ft.ToString()
                };
            }
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Formats hours as "Xh" or "X.5h"
    /// </summary>
    public class HoursFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return d % 1 == 0 ? $"{(int)d}h" : $"{d:F1}h";
            }
            return "0h";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Returns true if value is non-null, false if null. For Visibility triggers.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Converts CalculationViewModel to bold font weight if it matches the selected calculation
    /// </summary>
    public class EqualityToFontWeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] != null && values[1] != null)
                return values[0] == values[1] ? FontWeights.Bold : FontWeights.Normal;
            return FontWeights.Normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
