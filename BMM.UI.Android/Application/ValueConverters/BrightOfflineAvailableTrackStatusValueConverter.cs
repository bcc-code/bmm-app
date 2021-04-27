using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;

namespace BMM.UI.Droid.Application.ValueConverters
{
    public class BrightOfflineAvailableTrackStatusValueConverter : OfflineAvailableTrackStatusValueConverter
    {
        protected override int Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            var viewmodel = value.ViewModel;
            var item = value.Item;

            if (!(item is Track))
            {
                return 0;
            }

            var track = (Track)item;
            if (IsOfflineAvailable(track))
            {
                return Resource.Drawable.icon_downloaded_bright;
            }

            return base.Convert(value, targetType, parameter, culture);
        }
    }
}