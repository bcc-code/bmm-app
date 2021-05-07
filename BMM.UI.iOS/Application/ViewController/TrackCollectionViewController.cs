using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using System;
using BMM.Core.ValueConverters;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Views;
using UIKit;
using BMM.UI.iOS.Constants;
using CoreGraphics;
using MvvmCross.Binding.Combiners;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Plugin.Visibility;

namespace BMM.UI.iOS
{
    public partial class TrackCollectionViewController : BaseViewController<TrackCollectionViewModel>
    {
        public TrackCollectionViewController()
            : base("TrackCollectionViewController")
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ViewModel.IsConnectionOnline)
            {
                AddNavigationBarItemForOptions();
            }

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            CollectionTable.RefreshControl = refreshControl;

            var source = new DocumentsTableViewSource(CollectionTable);

            var set = this.CreateBindingSet<TrackCollectionViewController, TrackCollectionViewModel>();
            // set.Bind(this).For(c => c.Title).To(vm => vm.MyCollection).WithConversion<TrackCollectionNameConverter>();

            // set.Bind(OfflineAvailableTitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("AvailableOffline");
            // set.Bind(OfflineAvailableSubtitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("AvailableOfflineDownload");
            // set.Bind(OfflineAvailableSwitch).To(vm => vm.IsOfflineAvailable).OneWay();
            // set.Bind(OfflineAvailableButton).To(vm => vm.ToggleOfflineCommand);
            //
            // OfflineAvailableSwitch.ValueChanged += (object sender, EventArgs e) =>
            // {
            //     // Only update if the value is really changed :) Not just if you slide it on and off in one swipe.
            //     if (ViewModel.IsOfflineAvailable != OfflineAvailableSwitch.On)
            //     {
            //         ViewModel.ToggleOfflineCommand.Execute();
            //     }
            // };
            //
            // set.Bind(OfflineAvailableProgress).To(vm => vm.DownloadStatus);
            // set.Bind(DownloadingStatusLabel).To(vm => vm.DownloadingText);

            set.Bind(NameLabel).To(vm => vm.MyCollection.Name);

            DownloadButton.DownloadedImage = new UIImage("icon_tick");
            DownloadButton.NormalStateImage = new UIImage("icon_download");
            set.Bind(DownloadButton).To(vm => vm.ToggleOfflineCommand);
            set.Bind(DownloadButton).For(v => v.Label).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("AvailableOfflineDownload");
            set.Bind(DownloadButton).For(v => v.IsDownloading).To(vm => vm.IsDownloading);
            set.Bind(DownloadButton).For(v => v.IsDownloaded).To(vm => vm.IsOfflineAvailable);
            set.Bind(DownloadButton).For(v => v.DownloadProgress).To(vm => vm.DownloadStatus);

            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();

            set.Bind(OfflineBannerLabel).To(vm => vm.GlobalTextSource).WithConversion<MvxLanguageConverter>("OfflineBanner");
            HideOfflineBannerIfNecessary();

            // set.Bind(EmptyStateView).For(s => s.Hidden).To(vm => vm.IsEmpty).WithConversion<InvertedVisibilityConverter>();
            // set.Bind(PlaylistEmptyHeadlineLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("EmptyTitle");
            // set.Bind(PlaylistEmptyTextLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("EmptySubline");

            set.Bind(ShuffleButton).To(vm => vm.ShufflePlayCommand);
            set.Bind(ShuffleButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("ShufflePlay");

            // Fix the height of the table-header manually by watching the trigger
            ((TrackCollectionViewModel)DataContext).PropertyChanged += (sender, e) => { if (e.PropertyName == "IsDownloading") UpdateTableHeaderHeight(); };
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
            // DownloadingStatusView.Hidden = !((TrackCollectionViewModel)DataContext).IsDownloading;
            //
            // var headerFrame = PlaylistHeaderView.Frame;
            // var size = headerFrame.Size;
            // // If we'd remove the DownloadingStatusView we could use PlaylistHeaderView.SystemLayoutSizeFittingSize() ...
            // size.Height = DownloadingStatusView.Frame.Top + (DownloadingStatusView.Hidden ? 0 : DownloadingStatusView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize).Height);
            // headerFrame.Size = size;
            // PlaylistHeaderView.Frame = headerFrame;
            // CollectionTable.TableHeaderView = PlaylistHeaderView;
        }

        private void AddNavigationBarItemForOptions()
        {
            var sidebarButton = new UIBarButtonItem(
                UIImage.FromFile("icon_topbar_options_static.png"),
                UIBarButtonItemStyle.Plain,
                (object sender, EventArgs e) =>
                {
                    ViewModel.OptionCommand.Execute(ViewModel.MyCollection);
                }
            );

            NavigationItem.SetRightBarButtonItem(sidebarButton, true);
        }
    }
}