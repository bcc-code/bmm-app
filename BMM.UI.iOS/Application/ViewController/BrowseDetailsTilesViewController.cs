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
    public partial class BrowseDetailsTilesViewController : BaseViewController<BrowseDetailsTilesViewModel>, IHaveLargeTitle
    {
        public BrowseDetailsTilesViewController() : base(nameof(BrowseDetailsTilesViewController))
        {
        }

        public double? InitialLargeTitleHeight { get; set; }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl
            {
                TintColor = AppColors.RefreshControlTintColor
            };

            DocumentsTilesCollectionView.CollectionViewLayout = new FillWidthLayout();
            DocumentsTilesCollectionView.RefreshControl = refreshControl;
            refreshControl.TintColor = AppColors.RefreshControlTintColor;

            var source = new MvxCollectionViewSource(DocumentsTilesCollectionView, CoverWithTitleCollectionViewCell.Key);

            var nib = UINib.FromName(CoverWithTitleCollectionViewCell.Key, NSBundle.MainBundle);
            DocumentsTilesCollectionView.RegisterNibForCell(nib, CoverWithTitleCollectionViewCell.Key);

            DocumentsTilesCollectionView.Source = source;

            var set = this.CreateBindingSet<BrowseDetailsTilesViewController, BrowseDetailsTilesViewModel>();

            set.Bind(source)
                .For(s => s.ItemsSource)
                .To(vm => vm.Documents);

            set.Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand);

            set.Bind(refreshControl)
                .For(r => r.IsRefreshing)
                .To(vm => vm.IsRefreshing);

            set.Bind(refreshControl)
                .For(r => r.RefreshCommand)
                .To(vm => vm.ReloadCommand);

            set.Bind(this)
                .For(v => v.Title)
                .To(vm => vm.Title);

            set.Apply();
        }
    }
}