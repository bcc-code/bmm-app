using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class DeepLinkButtonCommandValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, MvxAsyncCommand>
    {
        protected override MvxAsyncCommand Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            DiscoverSectionHeader item = (DiscoverSectionHeader)value.Item;

            var deepLinkHandler = Mvx.IoCProvider.Resolve<IDeepLinkHandler>();

            return new ExceptionHandlingCommand(async () => { deepLinkHandler.Open(new Uri(item.Link)); });
        }
    }
}