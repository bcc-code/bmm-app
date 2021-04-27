using System;
using System.Globalization;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Converters;

namespace BMM.Core
{
    public class DownloadOptionSelectedCommandValueConverter: MvxValueConverter<MvxCommand<int>, MvxCommand<AutomaticDownloadCellWrapperViewModel>>
    {
        protected override MvxCommand<AutomaticDownloadCellWrapperViewModel> Convert(MvxCommand<int> value, Type targetType, object parameter, CultureInfo culture)
        {
            return new MvxCommand<AutomaticDownloadCellWrapperViewModel>(v =>
            {
                value.Execute(v.Item.Value);
            });
        }
    }
}
