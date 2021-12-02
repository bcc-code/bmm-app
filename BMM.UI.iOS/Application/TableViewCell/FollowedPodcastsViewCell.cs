using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Binding.BindingContext;
using System;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class FollowedPodcastsViewCell : BaseBMMTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("FollowedPodcastsViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("FollowedPodcastsViewCell");

        public FollowedPodcastsViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<FollowedPodcastsViewCell, CellWrapperViewModel<TrackCollection>>();
                set.Bind(TitleLabel).To(vm => vm.Item.Name);
                set.Apply();
            });
        }
    }
}