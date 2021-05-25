using System.Globalization;
using MvvmCross.Plugin.Visibility;
using MvvmCross.UI;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class VisibleOrHiddenValueConverter : MvxBaseVisibilityValueConverter<bool>
    {
        protected override MvxVisibility Convert(bool value, object parameter, CultureInfo culture)
        {
            // The MvxVisibility value converter returns collapsed when it's false. This converter returns hidden instead, meaning that the element still reserves the same space in the layout.
            return value ? MvxVisibility.Visible : MvxVisibility.Hidden;
        }
    }

    public class VisibilityAndValueConverter : MvxBaseVisibilityValueConverter<bool>
    {
        protected override MvxVisibility Convert(bool first, object parameter, CultureInfo culture)
        {
            bool second = parameter is bool ? (bool)parameter : false;
            return first && second ? MvxVisibility.Visible : MvxVisibility.Collapsed;
        }
    }
}