using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.MyContent;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Localization;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class FollowedPodcastsViewController : BaseViewController<FollowedPodcastsViewModel>
    {
        public FollowedPodcastsViewController() : base(nameof(FollowedPodcastsViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PodcastCollectionView.CollectionViewLayout = new FillWidthLayout();

            var source = new MvxCollectionViewSource(PodcastCollectionView, CoverWithTitleCollectionViewCell.Key);

            var nib = UINib.FromName(CoverWithTitleCollectionViewCell.Key, NSBundle.MainBundle);
            PodcastCollectionView.RegisterNibForCell(nib, CoverWithTitleCollectionViewCell.Key);

            PodcastCollectionView.Source = source;

            var set = this.CreateBindingSet<FollowedPodcastsViewController, FollowedPodcastsViewModel>();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.Documents);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.DocumentSelectedCommand);

            set.Bind(EmptyStateView).For(s => s.Hidden).To(vm => vm.ShowEmptyFollowedPodcasts).WithConversion<InvertedVisibilityConverter>();
            set.Bind(PlaylistEmptyHeadlineLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.FollowedPodcastsViewModel_EmptyTitle);
            set.Bind(PlaylistEmptyTextLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.FollowedPodcastsViewModel_EmptySubline);

            set.Apply();

            PodcastCollectionView.ReloadData();
            SetThemes();
        }

        private void SetThemes()
        {
            PlaylistEmptyHeadlineLabel.ApplyTextTheme(AppTheme.Title2);
            PlaylistEmptyTextLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}

