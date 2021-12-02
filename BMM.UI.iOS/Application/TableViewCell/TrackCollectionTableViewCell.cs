using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using System;
using BMM.Core.ValueConverters.TrackCollections;
using BMM.Core.ViewModels.Base;
using BMM.UI.iOS.Constants;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class TrackCollectionTableViewCell : BaseBMMTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("TrackCollectionTableViewCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("TrackCollectionTableViewCell");

        private const int SharePlaylistIconWidth = 24;
        private nfloat initialDownloadStatusImageWidth;
        private bool _isPlaylistSharedByMe;

        public TrackCollectionTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TrackCollectionTableViewCell, CellWrapperViewModel<TrackCollection>>();
                set.Bind(TitleLabel).To(vm => vm.Item.Name);

                set.Bind(SubtitleLabel)
                    .To(vm => vm.Item)
                    .WithConversion<TrackCollectionToListViewItemSubtitleLabelConverter>();

                set.Bind(this)
                    .For(v => v.IsPlaylistSharedByMe)
                    .To(vm => vm.Item)
                    .WithConversion<TrackCollectionToIsSharedByMeConverter>();
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

        public bool IsPlaylistSharedByMe
        {
            get => _isPlaylistSharedByMe;
            set
            {
                _isPlaylistSharedByMe = value;
                SharedPlaylistIconWidthConstraint.Constant = _isPlaylistSharedByMe ? SharePlaylistIconWidth : 0;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();
        }

        private void SetThemes()
        {
            TitleLabel.ApplyTextTheme(AppTheme.Title2);
            SubtitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}