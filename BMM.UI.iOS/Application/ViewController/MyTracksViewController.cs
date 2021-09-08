using MvvmCross.Binding.BindingContext;
using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Views;
using UIKit;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class MyTracksViewController : BaseViewController<MyTracksViewModel>
    {
        public MyTracksViewController()
            : base("MyTracksViewController")
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            CollectionTable.RefreshControl = refreshControl;

            var source = new DocumentsTableViewSource(CollectionTable);

            var set = this.CreateBindingSet<MyTracksViewController, MyTracksViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.MyCollection).WithConversion<TrackCollectionNameConverter>();

            set.Bind(OfflineAvailableTitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.MyTracksViewModel_AvailableOffline);
            set.Bind(OfflineAvailableSubtitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.MyTracksViewModel_AvailableOfflineDownload);
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
            set.Bind(DownloadingStatusLabel).To(vm => vm.DownloadingText);

            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();

            set.Bind(OfflineBannerLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.Global_OfflineBanner);
            HideOfflineBannerIfNecessary();

            set.Bind(EmptyStateView).For(s => s.Hidden).To(vm => vm.IsEmpty).WithConversion<InvertedVisibilityConverter>();
            set.Bind(PlaylistEmptyHeadlineLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.MyTracksViewModel_EmptyTitle);
            set.Bind(PlaylistEmptyTextLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.MyTracksViewModel_EmptySubline);

            set.Bind(ShuffleButton).To(vm => vm.ShufflePlayCommand);
            set.Bind(ShuffleButton).For("Title").To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.MyTracksViewModel_ShufflePlay);

            // Fix the height of the table-header manually by watching the trigger
            ((MyTracksViewModel)DataContext).PropertyChanged += (sender, e) => { if (e.PropertyName == "IsDownloading") UpdateTableHeaderHeight(); };
            UpdateTableHeaderHeight();

            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            CollectionTable.ReloadData();

            set.Apply();
        }

        private void HideOfflineBannerIfNecessary()
        {
            var offlineBannerVisibility = new OfflineBannerVisibilityValueConverter().Convert(ViewModel, null, null, null);
            if (!(bool)offlineBannerVisibility)
            {
                OfflineBannerViewHeightConstraint.Constant = 0;
                OfflineBannerView.Hidden = true;
            }
        }

        private void UpdateTableHeaderHeight()
        {
            DownloadingStatusView.Hidden = !((MyTracksViewModel)DataContext).IsDownloading;

            var headerFrame = PlaylistHeaderView.Frame;
            var size = headerFrame.Size;
            // If we'd remove the DownloadingStatusView we could use PlaylistHeaderView.SystemLayoutSizeFittingSize() ...
            size.Height = DownloadingStatusView.Frame.Top + (DownloadingStatusView.Hidden ? 0 : DownloadingStatusView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize).Height);
            headerFrame.Size = size;
            PlaylistHeaderView.Frame = headerFrame;
            CollectionTable.TableHeaderView = PlaylistHeaderView;
        }
    }
}