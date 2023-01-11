using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using CoreAnimation;
using MvvmCross.Platforms.Ios.Binding;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class TrackTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(TrackTableViewCell));
        private TrackState _trackState;

        public TrackTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<TrackTableViewCell, TrackPO>();
                set.Bind(TitleLabel).To(po => po.TrackTitle);
                set.Bind(TitleLabel)
                    .For(i => i.TextColor)
                    .To(po => po.TrackState)
                    .WithConversion<TrackToTitleColorConverter>();

                set.Bind(accessoryView).To(po => po.TrackSubtitle);
                set.Bind(accessoryView).For(i => i.TextColor).To(po => po.TrackState).WithConversion<TrackToSubtitleColorConverter>();
                set.Bind(metaLabel).To(po => po.TrackMeta);
                set.Bind(DownloadStatusImageView)
                    .For(i => i.ImagePath)
                    .To(po => po.TrackState)
                    .WithConversion<OfflineAvailableTrackStatusConverter>();
                set.Bind(DownloadStatusImageView)
                    .For(i => i.BindVisibility())
                    .To(po => po.TrackState)
                    .WithConversion<OfflineAvailableTrackValueConverter>();
                set.Bind(StatusImage)
                    .For(v => v.Image)
                    .To(po => po.TrackState)
                    .WithConversion<TrackToStatusImageConverter>();
                set.Bind(this)
                    .For(v => v.TrackState)
                    .To(po => po.TrackState);
                set.Bind(ReferenceButton)
                    .For(v => v.BindVisible())
                    .To(po => po.Track)
                    .WithConversion<TrackHasExternalRelationsValueConverter>();
                set.Bind(OptionsButton).To(po => po.OptionButtonClickedCommand);
                set.Bind(ReferenceButton).To(po => po.ShowTrackInfoCommand);
                set.Apply();

                SetThemes();
            });
        }

        public TrackState TrackState
        {
            get => _trackState;
            set
            {
                _trackState = value;
                
                if (_trackState.IsDownloading && !_trackState.IsDownloaded)
                    AddRotateAnimationToStatusImageView();
                else
                    DownloadStatusImageView.Layer.RemoveAllAnimations();
            }
        }

        private void AddRotateAnimationToStatusImageView()
        {
            DownloadStatusImageView.Layer.RemoveAllAnimations();
            var rotationAnimation = new CABasicAnimation();
            rotationAnimation.KeyPath = UIViewConstants.RotateAnimationKeyPath;
            rotationAnimation.To = new NSNumber(Math.PI * 2);
            rotationAnimation.Duration = 1;
            rotationAnimation.Cumulative = true;
            rotationAnimation.RepeatCount = float.MaxValue;
            DownloadStatusImageView.Layer.AddAnimation(rotationAnimation, "rotationAnimation");
        }

        private void SetThemes()
        {
            metaLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}