using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BedrockEditorPro.Utilities
{
    ///Source: https://github.com/Esri/arcgis-pro-sdk-community-samples/blob/master/Geometry/CoordinateSystemDialog/Common/ValueConverters.cs

    /// <summary>
    /// Returns the opposite or 'Not' of the input - True for False and False for True
    /// </summary>
    public class ReverseBoolConverter : IValueConverter
    {

        /// <summary>
        /// Convert True to False and False to True
        /// </summary>
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Converter cannot convert back.");
        }

    }

    /// <summary>
    /// Converts the given bool to a Visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {

        /// <summary>
        /// Convert True to Visible and False to Hidden
        /// </summary>
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;
            bool val = (bool)value;
            return val ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Converter cannot convert back.");
        }

    }
}
