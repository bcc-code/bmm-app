using System;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class PodcastsViewController : BaseViewController<PodcastsViewModel>
    {
        public PodcastsViewController() : base("PodcastsViewController")
        {
        }

        public override Type ParentViewControllerType => typeof(LibraryViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PodcastCollectionView.CollectionViewLayout = new FillWidthLayout();

            var refreshControl = new MvxUIRefreshControl();
            PodcastCollectionView.RefreshControl = refreshControl;
            refreshControl.TintColor = AppColors.RefreshControlTintColor;
            var source = new MvxCollectionViewSource(PodcastCollectionView, PodcastCollectionViewCell.Key);

            var nib = UINib.FromName(PodcastCollectionViewCell.Key, NSBundle.MainBundle);
            PodcastCollectionView.RegisterNibForCell(nib, PodcastCollectionViewCell.Key);

            PodcastCollectionView.Source = source;

            var set = this.CreateBindingSet<PodcastsViewController, PodcastsViewModel>();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.DocumentSelectedCommand).WithConversion(new DocumentSelectedCommandValueConverter());
            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            set.Apply();

            PodcastCollectionView.ReloadData();
        }
    }
}

