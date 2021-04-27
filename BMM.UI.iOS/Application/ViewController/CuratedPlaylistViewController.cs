using System;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using CoreGraphics;
using BMM.Core.ValueConverters;
using UIKit;
using MvvmCross.Platforms.Ios.Views;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Localization;

namespace BMM.UI.iOS
{
    public partial class CuratedPlaylistViewController : BaseViewController<CuratedPlaylistViewModel>
    {
        public CuratedPlaylistViewController()
            : base("CuratedPlaylistViewController")
        { }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            CuratedPlaylistTable.RefreshControl = refreshControl;

            var source = new NotSelectableDocumentsTableViewSource(CuratedPlaylistTable);

            var set = this.CreateBindingSet<CuratedPlaylistViewController, CuratedPlaylistViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.CuratedPlaylist.Title);
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();
            set.Bind(CuratedPlaylistCoverImageView).For(v => v.ImagePath).To(vm => vm.CuratedPlaylist.Cover);
            set.Bind(CuratedPlaylistBlurCoverImage).For(v => v.ImagePath).To(vm => vm.CuratedPlaylist.Cover);
            set.Bind(ShuffleButton).To(vm => vm.ShufflePlayCommand);
            set.Bind(ShuffleButton).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("ShufflePlay");;

            set.Bind(OfflineAvailableTitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("AvailableOffline");
            set.Bind(OfflineAvailableSubtitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("AvailableOfflineDownload");
            set.Bind(OfflineAvailableSwitch).To(vm => vm.IsOfflineAvailable).OneWay();
            set.Bind(OfflineAvailableButton).To(vm => vm.ToggleOfflineCommand);

            OfflineAvailableSwitch.ValueChanged += (object sender, EventArgs e) =>
            {
                // Only update if the value is really changed :) Not just if you slide it on and off in one swipe.
                if (ViewModel.IsOfflineAvailable != OfflineAvailableSwitch.On)
                {
                    ViewModel.ToggleOfflineCommand.Execute();
                }
            };

            set.Bind(OfflineAvailableProgress).To(vm => vm.DownloadStatus);
            set.Bind(DownloadingStatusView).For(v => v.Hidden).To(vm => vm.IsDownloading).WithConversion<InvertedVisibilityConverter>();
            set.Bind(DownloadingStatusLabel).To(vm => vm.DownloadingText);

            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);
            set.Apply();

            CuratedPlaylistTable.ReloadData();
            BlurBackground();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            CuratedPlaylistTable?.ResizeHeaderView();
        }

        private void BlurBackground()
        {
            var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
            var blurEffect = new UIVisualEffectView(blur);
            var screenWidth = UIScreen.MainScreen.Bounds.Width;

            var frame = new CGRect(0, 0, screenWidth, 520);
            blurEffect.Frame = frame;
            blurView.Add(blurEffect);
        }
    }
}