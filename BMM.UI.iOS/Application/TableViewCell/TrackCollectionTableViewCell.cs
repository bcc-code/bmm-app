using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using System;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.Base;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class TrackCollectionTableViewCell : MvxTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("TrackCollectionTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("TrackCollectionTableViewCell");

        private nfloat initialDownloadStatusImageWidth;

        public TrackCollectionTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TrackCollectionTableViewCell, CellWrapperViewModel<TrackCollection>>();
                set.Bind(TitleLabel).To(vm => vm.Item.Name);
                set.Bind(SubtitleLabel).To(vm => vm.Item.TrackCount).WithConversion<FormatConverter>("{0} Tracks");
                set.Apply();
            });

            // Initialize a manual binding to update the width of the DownloadStatus icon. Sadly, this cannot be done by binding.
            BindingContext.DataContextChanged += (object sender, EventArgs e) =>
            {
                if (DataContext == null)
                {
                    return;
                }

                // Find out the initial size of the DownloadStatusImageView ...
                if (DownloadStatusImageView != null && initialDownloadStatusImageWidth == 0f)
                {
                    foreach (var constraint in DownloadStatusImageView.Constraints)
                    {
                        if (constraint.FirstAttribute == NSLayoutAttribute.Width)
                        {
                            initialDownloadStatusImageWidth = constraint.Constant;
                            break;
                        }
                    }
                }

                var dataContext = (CellWrapperViewModel<Document>) DataContext;

                var trackCollectionsViewModel = (ContentBaseViewModel) dataContext.ViewModel;

                var trackCollection = (TrackCollection) dataContext.Item;

                var isOfflineAvailable = trackCollectionsViewModel.IsOfflineAvailable(trackCollection);

                if (isOfflineAvailable)
                {
                    foreach (var constraint in DownloadStatusImageView.Constraints)
                    {
                        if (constraint.FirstAttribute == NSLayoutAttribute.Width)
                        {
                            constraint.Constant = initialDownloadStatusImageWidth;
                        }
                    }

                    DownloadStatusImageView.NeedsUpdateConstraints();
                }
                else
                {
                    foreach (var constraint in DownloadStatusImageView.Constraints)
                    {
                        if (constraint.FirstAttribute == NSLayoutAttribute.Width)
                        {
                            constraint.Constant = 0f;
                        }
                    }

                    DownloadStatusImageView.NeedsUpdateConstraints();
                }
            };
        }
    }
}