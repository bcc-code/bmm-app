using System;
using BMM.Core.Implementations.Device;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using BMM.UI.iOS.Utils;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(TabName = Translations.MenuViewModel_Home,
        TabIconName = "icon_home",
        TabSelectedIconName = "icon_home_active",
        WrapInNavigationController = false)]
    public partial class ExploreNewestViewController : BaseViewController<ExploreNewestViewModel>, IHaveLargeTitle
    {
        private UIBarButtonItem _playbackHistoryButton;

        public double? InitialLargeTitleHeight { get; set; }

        public ExploreNewestViewController() : base(nameof(ExploreNewestViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetPlaybackHistoryButton();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            TrackTableView.RefreshControl = refreshControl;

            var source = new NotSelectableDocumentsTableViewSource(TrackTableView);

            var set = this.CreateBindingSet<ExploreNewestViewController, ExploreNewestViewModel>();
            
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand)
                .WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();
            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            set.Bind(_playbackHistoryButton)
                .To(vm => vm.NavigateToViewModelCommand)
                .CommandParameter(typeof(PlaybackHistoryViewModel));

            set.Apply();

            TrackTableView.ReloadData();

            ViewModel.RadioViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ShowBmmLive")
                    SetTableHeaderHeight();
            };

            if (!Mvx.IoCProvider.Resolve<IFeatureSupportInfoService>().SupportsSiri)
                return;
            
            await SiriUtils.DonateFromKaareShortcut();
            await SiriUtils.DonatePlayMusicShortcut();
        }

        private void SetPlaybackHistoryButton()
        {
            _playbackHistoryButton = new UIBarButtonItem(UIImage.FromBundle("icon_playback_history"), UIBarButtonItemStyle.Plain, null);
            _playbackHistoryButton.TintColor = AppColors.LabelPrimaryColor;
            NavigationItem.RightBarButtonItem = _playbackHistoryButton;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            SetTableHeaderHeight();
        }

        /// <summary>
        /// We need this because BMM Live widget can either be collapsed or visible whether there is an upcoming/live transmission or not.
        /// Even if we have a StackView which changes its height according to its children, TableView's height is not updated when the stream finishes and the widget disappears.
        /// </summary>
        private void SetTableHeaderHeight()
        {
            TrackTableView.ResizeHeaderView();
        }
    }
}