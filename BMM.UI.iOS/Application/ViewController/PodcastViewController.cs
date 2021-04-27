using System;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using CoreGraphics;
using BMM.Core.ValueConverters;
using UIKit;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Views;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class PodcastViewController : BaseViewController<PodcastViewModel>
    {
        public PodcastViewController()
            : base("PodcastViewController")
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddNavigationBarItemForOptions();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            PodcastTable.RefreshControl = refreshControl;

            var source = new NotSelectableDocumentsTableViewSource(PodcastTable);

            var set = this.CreateBindingSet<PodcastViewController, PodcastViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.Podcast.Title);
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.LoadMoreCommand).To(s => s.LoadMoreCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsFullyLoaded);
            set.Bind(OfflineBannerLabel).To(vm => vm.GlobalTextSource).WithConversion<MvxLanguageConverter>("OfflineBanner");
            set.Bind(PodcastCoverImageView).For(v => v.ImagePath).To(vm => vm.Podcast.Cover);
            set.Bind(PodcastBlurCoverImage).For(v => v.ImagePath).To(vm => vm.Podcast.Cover);
            set.Bind(FollowTitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("GetNotified");
            set.Bind(FollowSubtitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("GetNotifiedMessage");
            set.Bind(FollowingButton).To(vm => vm.ToggleFollowingCommand);
            set.Bind(FollowButton).To(vm => vm.ToggleFollowingCommand);
            set.Bind(FollowingTickImageView).For("Visibility").To(vm => vm.IsFollowing).WithConversion(new InvertedVisibilityConverter());
            set.Bind(FollowingButton).For("Visibility").To(vm => vm.IsFollowing).WithConversion(new InvertedVisibilityConverter());
            set.Bind(FollowButton).For("Visibility").To(vm => vm.IsFollowing);
            set.Bind(ButtonLabel).To(vm => vm.FollowButtonText);
            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);
            set.Apply();

            HideOfflineBannerIfNecessary();

            PodcastTable.ReloadData();
            BlurBackground();

            FollowButton.Layer.BorderColor = new CGColor(0, 1);
        }

        private void HideOfflineBannerIfNecessary()
        {
            var offlineBannerVisible = (bool) new OfflineBannerVisibilityValueConverter().Convert(ViewModel, null, null, null);
            if (!offlineBannerVisible)
            {
                OfflineBannerView.Hidden = true;
                OfflineBannerViewHeightConstraint.Constant = 0;
            }
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

        private void AddNavigationBarItemForOptions()
        {
            var sidebarButton = new UIBarButtonItem(
                UIImage.FromFile("icon_topbar_options_static.png"),
                UIBarButtonItemStyle.Plain,
                (object sender, EventArgs e) =>
                {
                    ViewModel.OptionCommand.Execute(ViewModel.Podcast);
                }
            );

            NavigationItem.SetRightBarButtonItem(sidebarButton, true);
        }
    }
}