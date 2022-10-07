using System;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;

namespace BMM.UI.iOS
{
    public partial class BrowseDetailsListViewController : BaseViewController<BrowseDetailsListViewModel>, IHaveLargeTitle
    {
        public BrowseDetailsListViewController() : base(nameof(BrowseDetailsListViewController))
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

            BrowseTableView.RefreshControl = refreshControl;

            var source = new NotSelectableDocumentsTableViewSource(BrowseTableView);

            var set = this.CreateBindingSet<BrowseDetailsListViewController, BrowseDetailsListViewModel>();

            set.Bind(source)
                .To(vm => vm.Documents);

            set.Bind(source)
                .For(s => s.LoadMoreCommand)
                .To(s => s.LoadMoreCommand);

            set.Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand);

            set.Bind(source)
                .For(s => s.IsFullyLoaded)
                .To(vm => vm.IsFullyLoaded);

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