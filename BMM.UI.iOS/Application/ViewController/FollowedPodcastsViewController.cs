using System;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.MyContent;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Localization;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class FollowedPodcastsViewController : BaseViewController<FollowedPodcastsViewModel>
    {
        public FollowedPodcastsViewController() : base("FollowedPodcastsViewController")
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

            var set = this.CreateBindingSet<FollowedPodcastsViewController, FollowedPodcastsViewModel>();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.DocumentSelectedCommand).WithConversion(new DocumentSelectedCommandValueConverter());

            set.Bind(EmptyStateView).For(s => s.Hidden).To(vm => vm.ShowEmptyFollowedPodcasts).WithConversion<InvertedVisibilityConverter>();
            set.Bind(PlaylistEmptyHeadlineLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("EmptyTitle");
            set.Bind(PlaylistEmptyTextLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>("EmptySubline");

            set.Apply();

            PodcastCollectionView.ReloadData();
        }
    }
}

