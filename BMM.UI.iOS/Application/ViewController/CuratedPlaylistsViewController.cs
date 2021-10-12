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
    public partial class CuratedPlaylistsViewController : BaseViewController<CuratedPlaylistsViewModel>, IHaveLargeTitle
    {
        public CuratedPlaylistsViewController() : base(nameof(CuratedPlaylistsViewController))
        {
        }

        public double? InitialLargeTitleHeight { get; set; }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CuratedPlaylistsCollectionView.CollectionViewLayout = new FillWidthLayout();

            var source = new MvxCollectionViewSource(CuratedPlaylistsCollectionView, CoverWithTitleCollectionViewCell.Key);

            var nib = UINib.FromName(CoverWithTitleCollectionViewCell.Key, NSBundle.MainBundle);
            CuratedPlaylistsCollectionView.RegisterNibForCell(nib, CoverWithTitleCollectionViewCell.Key);

            var refreshControl = new MvxUIRefreshControl();
            CuratedPlaylistsCollectionView.RefreshControl = refreshControl;
            refreshControl.TintColor = AppColors.RefreshControlTintColor;
            CuratedPlaylistsCollectionView.Source = source;

            var set = this.CreateBindingSet<CuratedPlaylistsViewController, CuratedPlaylistsViewModel>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.DocumentSelectedCommand).WithConversion(new DocumentSelectedCommandValueConverter());
            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            set.Apply();
        }
    }
}