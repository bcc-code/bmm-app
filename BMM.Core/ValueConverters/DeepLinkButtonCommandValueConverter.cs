using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Converters;
using MvvmCross.Navigation;

namespace BMM.Core.ValueConverters
{
    public class DeepLinkButtonCommandValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, MvxAsyncCommand>
    {
        protected override MvxAsyncCommand Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            DiscoverSectionHeader item = (DiscoverSectionHeader)value.Item;

            var deepLinkHandler = Mvx.IoCProvider.Resolve<IDeepLinkHandler>();

            return new ExceptionHandlingCommand(async () =>
            {
                if (item.Link.Contains("events"))
                {
                    var nasd = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
                    await nasd.Navigate<BrowseDetailsViewModel, IBrowseDetailsParameters>(new BrowseDetailsParameters(BrowseDetailsType.Events, item.Title));
                }
                //deepLinkHandler.OpenFromInsideOfApp(new Uri(item.Link));
            });
        }
    }
}