using System;
using System.Globalization;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class StorageSelectedCommandValueConverter : MvxValueConverter<MvxAsyncCommand<IFileStorage>, MvxCommand<CellWrapperViewModel<IFileStorage>>>
    {
        protected override MvxCommand<CellWrapperViewModel<IFileStorage>> Convert(MvxAsyncCommand<IFileStorage> value, Type targetType, object parameter, CultureInfo culture)
        {
            return new MvxCommand<CellWrapperViewModel<IFileStorage>>((v) =>
            {
                value.Execute(v.Item);
            });
        }
    }
}
