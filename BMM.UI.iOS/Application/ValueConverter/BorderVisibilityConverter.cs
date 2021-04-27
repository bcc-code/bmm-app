using System;
using System.Globalization;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class BorderVisibilityConverter : MvxValueConverter<bool, nfloat>
    {
        /// <summary>
        /// Remember that the parameter type has to a float value like this: 5f.
        /// Parameters are not typed.
        /// </summary>
        protected override nfloat Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value)
                return 0f;

            if (parameter is float nfloatParameter)
                return nfloatParameter;

            return 1f;
        }
    }
}