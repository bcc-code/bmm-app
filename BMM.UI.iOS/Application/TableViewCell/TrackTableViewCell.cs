using BMM.Core.Constants;
using BMM.Core.Helpers;
using MvvmCross.Binding.BindingContext;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews.Swipes;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.TableViewCell.Base;
using CoreAnimation;
using MvvmCross.Platforms.Ios.Binding;
using BMM.Core.Translation;

namespace BMM.UI.iOS
{
    public partial class TrackTableViewCell : SwipeableViewCell
    {
        public static readonly NSString Key = new(nameof(TrackTableViewCell));
        private TrackState _trackState;
        private string _offlineStateImage;

        public TrackTableViewCell(ObjCRuntime.NativeHandle handle)
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
                
                set.Bind(this)
                    .For(i => i.OfflineStateImage)
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

        public string OfflineStateImage
        {
            get => _offlineStateImage;
            set
            {
                _offlineStateImage = value;
                DownloadStatusImageView.ImagePath = _offlineStateImage;

                if (_offlineStateImage == ImageResourceNames.IconCheckmark.ToIosImageName())
                {
                    StatusImageWidthConstraint.Constant = 24;
                    DownloadStatusImageView.TintColor = AppColors.LabelOneColor;
                }
                else
                {
                    StatusImageWidthConstraint.Constant = 16;
                }
            }
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

        public override void SetupAndBindMenus()
        {
            var set = this.CreateBindingSet<TrackTableViewCell, TrackPO>();
            
            RightMenu.AddItem(CreateItem(set));
            LeftMenu.AddItem(CreateItem(set));
            
            set.Apply();
        }

        private static SwipeMenuSimpleItem CreateItem(
            MvxFluentBindingDescriptionSet<TrackTableViewCell, TrackPO> set)
        {
            var item = new SwipeMenuSimpleItem
            {
                TreatAsSingleAction = true
            };

            set.Bind(item.LabelTitle)
                .To(po => po.TextSource[Translations.QueueViewModel_Delete]);

            set.Bind(item)
                .For(i => i.ClickCommand)
                .To(po => po.DeleteFromQueueCommand);
            
            set.Bind(item)
                .For(i => i.FullSwipeCommand)
                .To(po => po.DeleteFromQueueCommand);
            
            return item;
        }
    }
}