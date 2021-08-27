using System;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(TabName = "Home", TabIconName = "icon_home", TabSelectedIconName = "icon_home_active", WrapInNavigationController = false)]
    public partial class ExploreNewestViewController : BaseViewController<ExploreNewestViewModel>, IHaveLargeTitle
    {
        public double? InitialLargeTitleHeight { get; set; }

        public ExploreNewestViewController() : base(nameof(ExploreNewestViewController))
        { }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            TrackTableView.RefreshControl = refreshControl;

            var source = new NotSelectableDocumentsTableViewSource(TrackTableView);

            var podcastSource = new PodcastNewestTrackTableViewSource(PodcastTrackListTableView);

            var set = this.CreateBindingSet<ExploreNewestViewController, ExploreNewestViewModel>();

            set.Bind(PodcastView).For(t => t.Hidden).To(vm => vm.FraKaareTeaserViewModel.ShowTeaser).WithConversion<InvertedVisibilityConverter>();
            set.Bind(podcastSource).To(vm => vm.FraKaareTeaserViewModel.Documents).WithConversion<DocumentListValueConverter>(ViewModel.FraKaareTeaserViewModel);
            set.Bind(podcastSource).For(s => s.SelectionChangedCommand).To(vm => vm.FraKaareTeaserViewModel.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(PodcastShowAllButton).To(vm => vm.FraKaareTeaserViewModel.ShowAllCommand);
            set.Bind(PodcastShowAllButton).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("ShowAll");
            set.Bind(PodcastTitleLabel).For(l => l.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("FraKaareHeader");
            set.Bind(FraKaarePlayRandomButton).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("PlayRandom");;
            set.Bind(FraKaarePlayRandomButton).To(vm => vm.FraKaareTeaserViewModel.PlayRandomCommand);

            set.Bind(AslaksenTeaser).For(t => t.Hidden).To(vm => vm.AslaksenTeaserViewModel.ShowTeaser).WithConversion<InvertedVisibilityConverter>();
            set.Bind(AslaksenShowAllButton).To(vm => vm.AslaksenTeaserViewModel.ShowAllCommand);
            set.Bind(AslaksenShowAllButton).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("ShowAll");
            set.Bind(AslaksenTitle).For(l => l.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("AslaksenTeaserHeader");
            set.Bind(AslaksenPlayRandomButton).To(vm => vm.AslaksenTeaserViewModel.PlayRandomCommand);
            set.Bind(AslaksenPlayRandomButton).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("PlayRandom");
            set.Bind(AslaksenPlayNewestButton).To(vm => vm.AslaksenTeaserViewModel.PlayNewestCommand);
            set.Bind(AslaksenPlayNewestButton).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("PlayNewest");

            set.Bind(ListHeaderLabel).For(l => l.Text).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("RecentTracks");

            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();
            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            set.Bind(BmmRadiosView).For(s => s.Hidden).To(vm => vm.RadioViewModel.ShowBmmLive).WithConversion<InvertedVisibilityConverter>();
            set.Bind(BmmRadioBroadcastingView).For(s => s.Hidden).To(vm => vm.RadioViewModel.IsBroadcasting).WithConversion<InvertedVisibilityConverter>();
            set.Bind(BmmRadioUpcomingView).For(s => s.Hidden).To(vm => vm.RadioViewModel.IsBroadcastUpcoming).WithConversion<InvertedVisibilityConverter>();
            set.Bind(BmmRadioPlay).To(vm => vm.RadioViewModel.PlayCommand);
            set.Bind(BmmRadioUpcomingPlay).To(vm => vm.RadioViewModel.PlayCommand);
            set.Bind(BmmRadioBroadcastingTitle).For(l => l.Text).To(vm => vm.RadioViewModel.Title).WithConversion<ToUppercaseConverter>();
            set.Bind(BmmRadioUpcomingTitle).For(l => l.Text).To(vm => vm.RadioViewModel.Title).WithConversion<ToUppercaseConverter>();
            set.Bind(BmmRadioBroadcastingDescription).For(l => l.Text).To(vm => vm.RadioViewModel.Track.Title);
            set.Bind(BmmRadioUpcomingDescription).For(l => l.Text).To(vm => vm.RadioViewModel.Track.Title);
            set.Bind(CountdownLabel).For(l => l.Text).To(vm => vm.RadioViewModel.TimeLeft).WithConversion<TimeSpanToCountdownValueConverter>();

            set.Apply();

            TrackTableView.ReloadData();

            ViewModel.RadioViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "ShowBmmLive")
                    SetTableHeaderHeight();
            };
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