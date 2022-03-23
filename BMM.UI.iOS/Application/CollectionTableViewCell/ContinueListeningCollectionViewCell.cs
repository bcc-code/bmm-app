using System;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ContinueListeningCollectionViewCell : MvxCollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(ContinueListeningCollectionViewCell));
        public static readonly UINib Nib = UINib.FromName(nameof(ContinueListeningCollectionViewCell), NSBundle.MainBundle);
        private DateTime? _date;
        private ITrackModel _currentPlayerTrack;
        private string _subtitle;

        public ContinueListeningCollectionViewCell(IntPtr handle): base(Key, handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<ContinueListeningCollectionViewCell, CellWrapperViewModel<ContinueListeningTile>>();
                
                set.Bind(BackgroundView)
                    .For(v => v.BackgroundColor)
                    .To(po => po.Item.BackgroundColor)
                    .WithConversion<HexStringToUiColorConverter>(AppColors.TileDefaultColor);

                set.Bind(BackgroundView)
                    .For(v => v.BindTap())
                    .To(po => po)
                    .WithConversion<TileClickedCommandValueConverter>();
                
                CoverImageView.ErrorAndLoadingPlaceholderImagePathForCover();
                set.Bind(CoverImageView)
                    .For(v => v.ImagePath)
                    .To(vm => vm.Item.CoverUrl)
                    .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);

                set.Bind(TitleLabel)
                    .To(vm => vm.Item.Label);
                
                set.Bind(SubtitleLabel)
                    .To(vm => vm.Item.Title);

                set.Bind(this)
                    .For(v => v.Date)
                    .To(vm => vm.Item.Date);
                
                set.Bind(DateLabel)
                    .To(vm => vm.Item.Date)
                    .WithConversion<DateTimeToPodcastPublishDateLabelValueConverter>();
                
                set.Bind(DayOfWeekLabel)
                    .To(vm => vm.Item.Date)
                    .WithConversion<DateTimeToPodcastPublishDayOfWeekLabelValueConverter>();

                set.Bind(this)
                    .For(v => v.Subtitle)
                    .To(vm => vm.Item.Subtitle);
                
                set.Bind(ProgressBarView)
                    .For(v => v.Percentage)
                    .To(vm => vm.Item.Percentage);
                
                set.Bind(ReferenceButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.Item.Track)
                    .WithConversion<TrackHasExternalRelationsValueConverter>();
                
                set.Bind(ReferenceButton)
                    .To(vm => vm)
                    .WithConversion<ShowTrackInfoCommandValueConverter>();
                
                set.Bind(DownloadedIcon)
                    .For(v => v.BindVisible())
                    .To(vm => vm.Item.Track)
                    .WithConversion<DownloadStatusDoneValueConverter>();
                
                set.Bind(OptionsButton)
                    .To(vm => vm)
                    .WithConversion<OptionButtonCommandValueConverter>();

                set.Bind(ShuffleButton)
                    .To(vm => vm)
                    .WithConversion<ShuffleButtonCommandValueConverter>();
                
                set.Bind(ShuffleButton)
                    .For(v => v.BindVisible())
                    .To(vm => vm.Item.ShufflePodcastId)
                    .WithConversion<HasValueToVisibleValueConverter>();
                
                set.Bind(this)
                    .For(v => v.CurrentPlayerTrack)
                    .To(vm => ((DocumentsViewModel)vm.ViewModel).CurrentTrack);

                set.Bind(TitleClickableArea)
                    .For(v => v.BindTap())
                    .WithConversion<ContinueListeningCommandValueConverter>();

                set.Bind(PlayButton).WithConversion<ContinueListeningCommandValueConverter>();
                set.Apply();
            });
        }
        
        public string Subtitle
        {
            get => _subtitle;
            set
            {
                _subtitle = value;
                
                if (_subtitle != null)
                    UpdateProgressBarSizeIfNeeded();

                RemainingLabel.Text = _subtitle;
            }
        }

        private void UpdateProgressBarSizeIfNeeded()
        {
            var newString = new NSString(_subtitle);
            var attribs = new UIStringAttributes
            {
                Font = RemainingLabel.Font
            };

            var size = newString.GetSizeUsingAttributes(attribs);
            UpdateSizeOfProgressBar(size.Width >= ContentWidthHelper.Frame.Width * 0.5f);
        }

        private void UpdateSizeOfProgressBar(bool isSmall)
        {
            float multipleFactor = isSmall
                ? 0.25f
                : 0.5f;

            ProgressBarWidthConstraint.Constant = ContentWidthHelper.Frame.Width * multipleFactor;
        }

        public ITrackModel CurrentPlayerTrack
        {
            get => _currentPlayerTrack;
            set
            {
                _currentPlayerTrack = value;
                if (_currentPlayerTrack == null)
                {
                    IsPlayingButton.Hidden = true;
                    return;
                }
                
                IsPlayingButton.Hidden = _currentPlayerTrack.Id !=
                                         ((ContinueListeningTile)((CellWrapperViewModel<Document>)DataContext).Item).Track.Id;
            }
        }

        public DateTime? Date
        {
            get => _date;
            set
            {
                _date = value;
                bool shouldShowProgressBar = _date == null;
                ProgressBarView.Hidden = !shouldShowProgressBar;
                RemainingLabel.Hidden = !shouldShowProgressBar;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            ProgressBarWidthConstraint.Constant = ContentWidthHelper.Frame.Width / 2;
            ShuffleButton.ApplyButtonStyle(AppTheme.ButtonTertiaryMediumOnColorFive);
            SubtitleLabel.ApplyTextTheme(AppTheme.Paragraph1OnColor2);
            TitleLabel.ApplyTextTheme(AppTheme.Title1OnColor1);
            DayOfWeekLabel.ApplyTextTheme(AppTheme.Subtitle3OnColor1);
            RemainingLabel.ApplyTextTheme(AppTheme.Subtitle3OnColor1);
            DateLabel.ApplyTextTheme(AppTheme.Subtitle3OnColor2);
        }
    }
}