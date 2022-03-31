using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using System;
using System.ComponentModel;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ValueConverters.TrackCollections;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Views;
using UIKit;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class TrackCollectionViewController : BaseViewController<TrackCollectionViewModel>
    {
        private bool _isSharingStatusIconVisible;

        public TrackCollectionViewController()
            : base(nameof(TrackCollectionViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public bool IsSharingStatusIconVisible
        {
            get => _isSharingStatusIconVisible;
            set
            {
                _isSharingStatusIconVisible = value;
                PlaylistIcon.SetHiddenIfNeeded(!_isSharingStatusIconVisible);
                CollectionTable.ResizeHeaderView();
            }
        }

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

            set.Bind(NameLabel).To(vm => vm.MyCollection.Name);

            set
                .Bind(PlaylistState)
                .To(vm => vm.MyCollection)
                .WithConversion<TrackCollectionToSharingStateConverter>();

            set
                .Bind(this)
                .For(v => v.IsSharingStatusIconVisible)
                .To(vm => vm.MyCollection)
                .WithConversion<TrackCollectionToPlaylistStatusIconIsVisibleConverter>();

            DownloadButton.DownloadedImage = UIImage.FromBundle("TickIcon");
            DownloadButton.NormalStateImage = UIImage.FromBundle("IconDownload");
            set.Bind(DownloadButton).To(vm => vm.ToggleOfflineCommand);
            set.Bind(DownloadButton).For(v => v.Label).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.TrackCollectionViewModel_AvailableOfflineDownload);
            set.Bind(DownloadButton).For(v => v.IsDownloading).To(vm => vm.IsDownloading);
            set.Bind(DownloadButton).For(v => v.IsDownloaded).To(vm => vm.IsDownloaded);
            set.Bind(DownloadButton).For(v => v.DownloadProgress).To(vm => vm.DownloadStatus);

            set.Bind(TrackCountLabel).To(vm => vm.TrackCountString);

            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();

            set.Bind(OfflineBannerLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.Global_OfflineBanner);
            HideOfflineBannerIfNecessary();

            set.Bind(ShuffleButton).To(vm => vm.ShufflePlayCommand);
            set.Bind(ShuffleButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.TrackCollectionViewModel_ShufflePlay);
            set.Bind(ShuffleButton).For(v => v.Hidden).To(vm => vm.IsEmpty);

            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            CollectionTable.ReloadData();

            set.Apply();

            SetThemes();
            DownloadButton.UpdateCurrentState(true);
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

        private void SetThemes()
        {
            NameLabel.ApplyTextTheme(AppTheme.Heading2);
            PlaylistState.ApplyTextTheme(AppTheme.Subtitle1Label2);
            ShuffleButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            DownloadButton.ApplyButtonStyle(AppTheme.ButtonSecondaryMedium);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Since the Name of the playlist could grow or shrink the header size is adjusted
            if (e.PropertyName == nameof(TrackCollectionViewModel.MyCollection))
                CollectionTable.ResizeHeaderView();
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

        private void AddNavigationBarItemForOptions()
        {
            var sidebarButton = new UIBarButtonItem(
                new UIImage("icon_options"),
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