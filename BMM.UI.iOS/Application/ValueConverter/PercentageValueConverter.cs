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
            Debug.Assert(parameter is MiniPlayerViewModel, "parameter has to be a 'MiniPlayerViewModel'");
            var miniPlayerViewModel = (MiniPlayerViewModel)parameter;
            var max = miniPlayerViewModel.Duration;

            var percentage = 1f / max * value;
            return percentage;
        }
    }
}