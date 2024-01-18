using System;
using System.Diagnostics;
using System.Globalization;
using BMM.Core.ViewModels;
using MvvmCross.Converters;

namespace BMM.UI.iOS
{
    public class PercentageValueConverter: MvxValueConverter<long, float>
    {
        protected override float Convert(long value, Type targetType, object parameter, CultureInfo culture)
        {
            var playerViewModel = (PlayerBaseViewModel)parameter;
            var max = playerViewModel.Duration;

            var percentage = 1f / max * value;
            return percentage;
        }
    }
}