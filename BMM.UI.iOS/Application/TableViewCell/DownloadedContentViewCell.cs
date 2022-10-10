using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Binding.BindingContext;
using System;
using BMM.Core.Models.POs.TrackCollections;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class DownloadedContentViewCell : BaseBMMTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("DownloadedContentViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("DownloadedContentViewCell");

        public DownloadedContentViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<DownloadedContentViewCell, TrackCollectionPO>();
                set.Bind(TitleLabel).To(vm => vm.TrackCollection.Name);

                set.Apply();
            });
        }
    }
}