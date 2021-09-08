using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.MyContent;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Localization;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class DownloadedFollowedPodcastsViewController : BaseViewController<DownloadedFollowedPodcastsViewModel>
    {
        public DownloadedFollowedPodcastsViewController() : base(nameof(DownloadedFollowedPodcastsViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PodcastCollectionView.CollectionViewLayout = new FillWidthLayout();

            var source = new MvxCollectionViewSource(PodcastCollectionView, PodcastCollectionViewCell.Key);

            var nib = UINib.FromName(PodcastCollectionViewCell.Key, NSBundle.MainBundle);
            PodcastCollectionView.RegisterNibForCell(nib, PodcastCollectionViewCell.Key);

            PodcastCollectionView.Source = source;

            var set = this.CreateBindingSet<DownloadedFollowedPodcastsViewController, DownloadedFollowedPodcastsViewModel>();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.DocumentSelectedCommand).WithConversion(new DocumentSelectedCommandValueConverter());
            set.Bind(OfflineBannerLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.Global_OfflineBanner);

            set.Bind(EmptyStateView).For(s => s.Hidden).To(vm => vm.ShowEmptyFollowedPodcasts).WithConversion<InvertedVisibilityConverter>();
            set.Bind(PlaylistEmptyHeadlineLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.DownloadedFollowedPodcastsViewModel_EmptyTitle);
            set.Bind(PlaylistEmptyTextLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.DownloadedFollowedPodcastsViewModel_EmptySubline);

            set.Apply();

            PodcastCollectionView.ReloadData();
        }
    }
}

