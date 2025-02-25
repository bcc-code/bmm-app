using System.Globalization;
using BMM.Core.Models.Enums;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters;

public class ActionButtonTypeToPlayIconVisibilityValueConverter : MvxValueConverter<AchievementActionButtonType, bool>
{
    protected override bool Convert(AchievementActionButtonType value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == AchievementActionButtonType.PlayNext)
            return true;

        return false;
    }
}