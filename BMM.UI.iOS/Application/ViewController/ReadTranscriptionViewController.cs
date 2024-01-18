using Accounts;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Combiners;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class ReadTranscriptionViewController : BaseViewController<ReadTranscriptionViewModel>
    {
        private BaseSimpleTableViewSource _source;

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
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<ReadTranscriptionViewController, ReadTranscriptionViewModel>();
           
            _source = new BaseSimpleTableViewSource(TranscriptionsTableView, ReadTranscriptionTableViewCell.Key);
            set.Bind(_source).To(vm => vm.Transcriptions);
            
           set.Bind(CloseButtonContainer)
               .For(v => v.BindTap())
               .To(vm => vm.CloseCommand);
           
           set.Bind(Header)
               .For(v => v.Text)
               .To(vm => vm.TextSource[Translations.HighlightedTextTrackViewModel_AutoTranscribed]);
           
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
           
            set.Apply();
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
            Header.TextColor = AppColors.UtilityAutoColor;
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

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}