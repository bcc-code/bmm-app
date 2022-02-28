using System;
using BMM.Api.Implementation.Models;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public class BaseTrackTableViewCell : BaseBMMTableViewCell
    {
        public BaseTrackTableViewCell(IntPtr handle): base(handle) {}

        protected virtual bool DownloadStatusVisible(CellWrapperViewModel<Document> dataContext)
        {
            var valueConverter = new OfflineAvailableTrackStatusConverter();
            return valueConverter.Convert(dataContext, null, null, null) != null;
        }

        protected virtual bool ReferenceButtonVisible(CellWrapperViewModel<Document> dataContext)
        {
            var valueConverter = new TrackHasExternalRelationsValueConverter();
            return (bool) valueConverter.Convert(dataContext.Item, null, null, null);
        }
    }
}