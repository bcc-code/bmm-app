using MvvmCross.Binding.BindingContext;
using BMM.Core.Constants;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.ValueConverters.TrackCollections;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;

namespace BMM.UI.iOS
{
    public partial class TrackCollectionTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(TrackCollectionTableViewCell));

        private const int SharePlaylistIconWidth = 24;
        private nfloat _initialDownloadStatusImageWidth;
        private bool _isPlaylistSharedByMe;
        private bool _useLikeIcon;

        public TrackCollectionTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TrackCollectionTableViewCell, TrackCollectionPO>();
                set.Bind(TitleLabel).To(po => po.TrackCollection.Name);

                set.Bind(SubtitleLabel)
                    .To(po => po.TrackCollection)
                    .WithConversion<TrackCollectionToListViewItemSubtitleLabelConverter>();

                set.Bind(this)
                    .For(v => v.IsPlaylistSharedByMe)
                    .To(vm => vm.TrackCollection)
                    .WithConversion<TrackCollectionToIsSharedByMeConverter>();

                set.Bind(this)
                    .For(v => v.UseLikeIcon)
                    .To(vm => vm.TrackCollection.UseLikeIcon);
                
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
                if (DownloadStatusImageView != null && _initialDownloadStatusImageWidth == 0f)
                {
                    foreach (var constraint in DownloadStatusImageView.Constraints)
                    {
                        if (constraint.FirstAttribute == NSLayoutAttribute.Width)
                        {
                            _initialDownloadStatusImageWidth = constraint.Constant;
                            break;
                        }
                    }
                }

                var trackCollectionPO = (TrackCollectionPO)DataContext;

                if (trackCollectionPO.IsAvailableOffline)
                {
                    foreach (var constraint in DownloadStatusImageView.Constraints)
                    {
                        if (constraint.FirstAttribute == NSLayoutAttribute.Width)
                        {
                            constraint.Constant = _initialDownloadStatusImageWidth;
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
        
        public bool UseLikeIcon
        {
            get => _useLikeIcon;
            set
            {
                _useLikeIcon = value;
                IconImageView.Image = _useLikeIcon
                    ? UIImage.FromBundle(ImageResourceNames.IconUnliked.ToStandardIosImageName())
                    : UIImage.FromBundle(ImageResourceNames.IconPlaylist.ToStandardIosImageName());
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