using System;
using System.ComponentModel;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using CoreGraphics;
using BMM.Core.ValueConverters;
using UIKit;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Views;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Helpers;
using Foundation;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class PodcastViewController : BaseViewController<PodcastViewModel>
    {
        public PodcastViewController()
            : base(nameof(PodcastViewController))
        { }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddNavigationBarItemForOptions();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            PodcastTable.RefreshControl = refreshControl;

            TitelLabel.ApplyTextTheme(AppTheme.Heading2);
            FollowButton.ApplyButtonStyle(AppTheme.ButtonSecondaryMedium);
            FollowingButton.ApplyButtonStyle(AppTheme.ButtonSecondaryMedium);
            PlayButton.ApplyButtonStyle(AppTheme.ButtonPrimary);

            var source = new NotSelectableDocumentsTableViewSource(PodcastTable);

            var set = this.CreateBindingSet<PodcastViewController, PodcastViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.Podcast.Title);
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.LoadMoreCommand).To(s => s.LoadMoreCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsFullyLoaded);
            set.Bind(OfflineBannerLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.Global_OfflineBanner);
            set.Bind(PodcastCoverImageView).For(v => v.ImagePath).To(vm => vm.Podcast.Cover);

            set.Bind(TitelLabel).To(vm => vm.Podcast.Title);

            set.Bind(FollowingButton).For(v => v.Hidden).To(vm => vm.IsFollowing).WithConversion<InvertedVisibilityConverter>();
            set.Bind(FollowingButton).To(vm => vm.ToggleFollowingCommand);
            set.Bind(FollowingButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.PodcastViewModel_Following);

            set.Bind(FollowButton).For(v => v.Hidden).To(vm => vm.IsFollowing);
            set.Bind(FollowButton).To(vm => vm.ToggleFollowingCommand);
            set.Bind(FollowButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.PodcastViewModel_Follow);

            set.Bind(PlayButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.DocumentsViewModel_Play);
            set.Bind(PlayButton).To(vm => vm.PlayCommand);

            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            set.Apply();

            HideOfflineBannerIfNecessary();

            PodcastTable.ResizeHeaderView();

            FollowButton.Layer.BorderColor = new CGColor(0, 1);
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PodcastTable.ResizeHeaderView();
        }

        private void HideOfflineBannerIfNecessary()
        {
            var offlineBannerVisible = (bool)new OfflineBannerVisibilityValueConverter().Convert(ViewModel, null, null, null);
            if (!offlineBannerVisible)
            {
                OfflineBannerView.Hidden = true;
                OfflineBannerViewHeightConstraint.Constant = 0;
            }
        }

        private void AddNavigationBarItemForOptions()
        {
            var sidebarButton = new UIBarButtonItem(
                new UIImage("icon_options"),
                UIBarButtonItemStyle.Plain,
                (object sender, EventArgs e) => { ViewModel.OptionCommand.Execute(ViewModel.Podcast); }
            );

            NavigationItem.SetRightBarButtonItem(sidebarButton, true);
        }
    }
}