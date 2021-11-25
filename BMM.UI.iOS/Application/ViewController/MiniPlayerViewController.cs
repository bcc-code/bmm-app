using System;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Combiners;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class MiniPlayerViewController : BaseViewController<MiniPlayerViewModel>
    {
        public MiniPlayerViewController() : base(nameof(MiniPlayerViewController))
        { }

        public override System.Type ParentViewControllerType => typeof(ContainmentViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ProgressBar.ProgressColor = AppColors.ColorPrimary;

            var set = this.CreateBindingSet<MiniPlayerViewController, MiniPlayerViewModel>();
            set.Bind(ProgressBar).For(s => s.Progress).To(vm => vm.SliderPosition)
                .WithConversion<PercentageValueConverter>(ViewModel);
            set.Bind(ProgressBar).For(s => s.Alpha)
                .ByCombining(new MvxInvertedValueCombiner(), vm => vm.IsSeekingDisabled)
                .WithConversion<BoolToNfloatConverter>();

            set.Bind(PlayerStatusButton).To(vm => vm.PlayPauseCommand);
            set.Bind(PlayerStatusButton).For(v => v.Selected).To(vm => vm.IsPlaying);
            set.Bind(TrackTitleLabel).To(vm => vm.CurrentTrack).WithConversion<TrackToTitleValueConverter>(ViewModel);
            set.Bind(TrackSubtitleLabel).To(vm => vm.CurrentTrack).WithConversion<TrackToSubtitleValueConverter>(ViewModel);
            set.Bind(ShowPlayerButton).To(vm => vm.OpenPlayerCommand);

            CoverView.ErrorAndLoadingPlaceholderImagePathForCover();
            set.Bind(CoverView)
                .For(v => v.ImagePath)
                .To(vm => vm.CurrentTrack.ArtworkUri)
                .WithConversion<CoverUrlToFallbackImageValueConverter>(IosConstants.CoverPlaceholderImage);

            set.Apply();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            MiniPlayerHeight = View!.Frame.Height;
        }

        public static nfloat MiniPlayerHeight { get; set; }
    }
}