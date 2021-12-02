using System;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using BMM.Core.ValueConverters;
using MvvmCross.Platforms.Ios.Views;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Extensions;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class CuratedPlaylistViewController : BaseViewController<CuratedPlaylistViewModel>
    {
        public CuratedPlaylistViewController()
            : base(nameof(CuratedPlaylistViewController))
        { }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            CuratedPlaylistTable.RefreshControl = refreshControl;

            var source = new NotSelectableDocumentsTableViewSource(CuratedPlaylistTable);

            TitleLabel.ApplyTextTheme(AppTheme.Heading2);
            DescriptionLabel.ApplyTextTheme(AppTheme.Paragraph2);
            ShuffleButton.ApplyButtonStyle(AppTheme.ButtonPrimary.Value);
            DownloadButton.ApplyButtonStyle(AppTheme.ButtonSecondaryMedium);
            DownloadButton.DownloadedImage = new UIImage("icon_tick");
            DownloadButton.NormalStateImage = new UIImage("icon_download");
            TrackCountLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);

            var set = this.CreateBindingSet<CuratedPlaylistViewController, CuratedPlaylistViewModel>();
            set.Bind(this).For(c => c.Title).To(vm => vm.CuratedPlaylist.Title);
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();

            set.Bind(CuratedPlaylistCoverImageView).For(v => v.ImagePath).To(vm => vm.CuratedPlaylist.Cover);
            set.Bind(TitleLabel).To(vm => vm.CuratedPlaylist.Title);
            set.Bind(DescriptionLabel).To(vm => vm.CuratedPlaylist.Description);

            set.Bind(ShuffleButton).To(vm => vm.ShufflePlayCommand);
            set.Bind(ShuffleButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.TrackCollectionViewModel_ShufflePlay);;
            set.Bind(TrackCountLabel).To(vm => vm.TrackCountString);

            set.Bind(DownloadButton).To(vm => vm.ToggleOfflineCommand);
            set.Bind(DownloadButton).For(v => v.Label).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.TrackCollectionViewModel_AvailableOfflineDownload);
            set.Bind(DownloadButton).For(v => v.IsDownloading).To(vm => vm.IsDownloading);
            set.Bind(DownloadButton).For(v => v.IsDownloaded).To(vm => vm.IsDownloaded);
            set.Bind(DownloadButton).For(v => v.DownloadProgress).To(vm => vm.DownloadStatus);

            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);
            set.Apply();

            CuratedPlaylistTable.ReloadData();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            CuratedPlaylistTable?.ResizeHeaderView();
        }
    }
}