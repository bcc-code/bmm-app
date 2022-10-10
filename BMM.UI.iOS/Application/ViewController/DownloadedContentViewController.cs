using MvvmCross.Binding.BindingContext;
using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.MyContent;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Views;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class DownloadedContentViewController : BaseViewController<DownloadedContentViewModel>
    {
        public DownloadedContentViewController()
            : base(nameof(DownloadedContentViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            MyCollectionTable.RefreshControl = refreshControl;

            var source = new DocumentsTableViewSource(MyCollectionTable);

            var set = this.CreateBindingSet<DownloadedContentViewController, DownloadedContentViewModel>();
            set.Bind(source).To(vm => vm.Documents);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();
            set.Bind(OfflineBannerLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.Global_OfflineBanner);

            set.Bind(NoOfflineTrackCollectionView).For(s => s.Hidden).To(vm => vm.IsEmpty).WithConversion<InvertedVisibilityConverter>();
            set.Bind(NoOfflineTrackCollectionHeadlineLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.DownloadedContentViewModel_EmptyTitle);
            set.Bind(NoOfflineTrackCollectionTextLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.DownloadedContentViewModel_EmptySubline);

            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            set.Apply();

            MyCollectionTable.ReloadData();
            SetThemes();
        }

        private void SetThemes()
        {
            NoOfflineTrackCollectionHeadlineLabel.ApplyTextTheme(AppTheme.Title2);
            NoOfflineTrackCollectionTextLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}