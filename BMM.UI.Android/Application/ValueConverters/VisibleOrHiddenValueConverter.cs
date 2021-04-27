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
}