using BMM.Core.Constants;
using BMM.Core.Models.Player.Lyrics;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Combiners;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class ReadTranscriptionViewController : BaseViewController<ReadTranscriptionViewModel>
    {
        private const int TableViewBottomOffset = 120; 
        private TranscriptionsTableViewSource _source;
        private IMvxInteraction<int> _adjustScrollPositionInteraction;
        private int _lastPosition;
        private bool _initialized;
        private bool _isAutoTranscribed;
        private LyricsLink _lyricsLink;

        public ReadTranscriptionViewController() : base(null)
        {
        }

        public ReadTranscriptionViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss
            };
            NavigationController.NavigationBarHidden = true;

            SetThemes();
            Bind();
            TranscriptionsTableView.ContentInset = new UIEdgeInsets(0, 0, TableViewBottomOffset, 0);
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<ReadTranscriptionViewController, ReadTranscriptionViewModel>();
           
            _source = new TranscriptionsTableViewSource(TranscriptionsTableView);
            set.Bind(_source).To(vm => vm.Transcriptions);
            
           set.Bind(CloseButtonContainer)
               .For(v => v.BindTap())
               .To(vm => vm.CloseCommand);

           set.Bind(Header)
               .To(vm => vm.HeaderText);

           set.Bind(this)
               .For(v => v.IsAutoTranscribed)
               .To(vm => vm.IsAutoTranscribed);
           
           set.Bind(this)
               .For(v => v.LyricsLink)
               .To(vm => vm.LyricsLink);
           
           set.Bind(HeaderContainerView)
               .For(v => v.BindTap())
               .To(vm => vm.HeaderClickedCommand);
           
           set.Bind(PlayerStatusButton).To(vm => vm.PlayPauseCommand);
           set.Bind(PlayerStatusButton).For(v => v.Selected).To(vm => vm.IsPlaying);
           set.Bind(TrackTitleLAbel).To(vm => vm.CurrentTrack).WithConversion<TrackToTitleValueConverter>(ViewModel);
           set.Bind(TrackSubtitleLabel).To(vm => vm.CurrentTrack).WithConversion<TrackToSubtitleValueConverter>(ViewModel);
           
           CoverView.ErrorAndLoadingPlaceholderImagePathForCover();
            
           set.Bind(CoverView)
               .For(v => v.ImagePath)
               .To(vm => vm.CurrentTrack.ArtworkUri)
               .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);
           
           set.Bind(ProgressBar).For(s => s.Progress).To(vm => vm.SliderPosition)
               .WithConversion<PercentageValueConverter>(ViewModel);

           set.Bind(ProgressBar).For(s => s.Alpha)
               .ByCombining(new MvxInvertedValueCombiner(), vm => vm.IsSeekingDisabled)
               .WithConversion<BoolToNfloatConverter>();
           
           set.Bind(this)
               .For(f => f.AdjustScrollPositionInteraction)
               .To(vm => vm.AdjustScrollPositionInteraction)
               .OneWay();

            set.Apply();
        }

        public LyricsLink LyricsLink
        {
            get => _lyricsLink;
            set
            {
                _lyricsLink = value;
                SetIcon();
            }
        }

        public bool IsAutoTranscribed
        {
            get => _isAutoTranscribed;
            set
            {
                _isAutoTranscribed = value;
                SetHeaderColor();
                SetIcon();
            }
        }

        private void SetIcon()
        {
            if (_isAutoTranscribed)
            {
                ImageIcon.Image = UIImage.FromBundle(ImageResourceNames.IconAI.ToStandardIosImageName());
                ImageIcon.TintColor = AppColors.UtilityAutoColor;
            }
            else
            {
                ImageIcon.TintColor = AppColors.LabelOneColor;
                ImageIcon.Image = UIImage.FromBundle(LyricsLink?.LyricsLinkType == LyricsLinkType.SongTreasures
                    ? ImageResourceNames.ImageSongTreasures.ToStandardIosImageName()
                    : ImageResourceNames.IconInfo.ToStandardIosImageName());
            }
        }

        public IMvxInteraction<int> AdjustScrollPositionInteraction
        {
            get => _adjustScrollPositionInteraction;
            set
            {
                if (_adjustScrollPositionInteraction != null)
                    _adjustScrollPositionInteraction.Requested -= OnAdjustScrollPositionInteractionRequested;

                _adjustScrollPositionInteraction = value;
                _adjustScrollPositionInteraction.Requested += OnAdjustScrollPositionInteractionRequested;
            }
        }
        
        protected override void AttachEvents()
        {
            base.AttachEvents();
            if (_adjustScrollPositionInteraction != null)
            {
                _adjustScrollPositionInteraction.Requested -= OnAdjustScrollPositionInteractionRequested;
                _adjustScrollPositionInteraction.Requested += OnAdjustScrollPositionInteractionRequested;
            }
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();

            if (_adjustScrollPositionInteraction != null)
                _adjustScrollPositionInteraction.Requested -= OnAdjustScrollPositionInteractionRequested;
        }
        
        private void OnAdjustScrollPositionInteractionRequested(object sender, MvxValueEventArgs<int> e)
        {
            if (_lastPosition == e.Value)
                return;
            
            _lastPosition = e.Value;
            
            BeginInvokeOnMainThread(() =>
            {
                TranscriptionsTableView.ScrollToRow(NSIndexPath.FromRowSection(new IntPtr(e.Value), 0), 
                    UITableViewScrollPosition.Middle,
                    _initialized);
                
                _initialized = true;
            });
        }
        
        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            CloseButtonContainer.Layer.BorderColor = AppColors.SeparatorColor.CGColor;
            PlayerView.Layer.BorderColor = AppColors.SeparatorColor.CGColor;
        }
        
        private void SetThemes()
        {
            ProgressBar.ProgressColor = AppColors.TintColor;
            Header.ApplyTextTheme(AppTheme.Paragraph2);
            SetHeaderColor();
            CloseButtonContainer.Layer.BorderWidth = 0.5f;
            CloseButtonContainer.Layer.ShadowRadius = 8;
            CloseButtonContainer.Layer.ShadowOffset = CGSize.Empty;
            CloseButtonContainer.Layer.ShadowOpacity = 0.1f;
            PlayerView.Layer.BorderWidth = 0.5f;
            PlayerViewShadowContainer.Layer.ShadowRadius = 8;
            PlayerViewShadowContainer.Layer.ShadowOffset = CGSize.Empty;
            PlayerViewShadowContainer.Layer.ShadowOpacity = 0.1f;
            PlayerView.ClipsToBounds = true;
            TrackTitleLAbel.ApplyTextTheme(AppTheme.Title2);
            TrackSubtitleLabel.ApplyTextTheme(AppTheme.Subtitle3Label2);
            View!.BringSubviewToFront(PlayerView);
        }
        
        private void SetHeaderColor()
        {
            Header.TextColor = _isAutoTranscribed
                ? AppColors.UtilityAutoColor
                : AppColors.LabelOneColor;
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}