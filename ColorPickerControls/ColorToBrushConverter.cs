using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Paradox_Editor.ColorPickerControls;

/// <summary>
/// Converts the Color value to the SolidColorBrush.
/// </summary>
[ValueConversion(/*sourceType*/ typeof(Color), /*targetType*/ typeof(Brush))]
public class ColorToBrushConverter : IValueConverter
{
    #region IValueConverter Members
    /// <summary>
    /// Converts an Integer value to Visibility.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Provides the value to compare with the <paramref name="value"/>.
    /// Can be <c>null</c>.</param>
    /// <param name="culture"><see cref="M:System.Convert.ToInt64"/> convertion culture.</param>
    /// <returns>
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(Brush))
            return value;
        if (value == null || value == DependencyProperty.UnsetValue
                          || value.GetType() != typeof(Color))
            return Brushes.Transparent;
        var color = (Color)value;
        return new SolidColorBrush(color);
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    #endregion IValueConverter Members
}