using System;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.MyContent;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(TabName = "Favorites", TabIconName = "icon_favorites", TabSelectedIconName = "icon_favorites_active", WrapInNavigationController = false)]
    public partial class MyContentViewController : BaseViewController<MyContentViewModel>, IHaveLargeTitle
    {
        public double? InitialLargeTitleHeight { get; set; }

        public MyContentViewController()
            : base("MyContentViewController")
        { }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddNavigationBarItemForAddPlaylist();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            MyCollectionTable.RefreshControl = refreshControl;

            var source = new DocumentsTableViewSource(MyCollectionTable);

            var set = this.CreateBindingSet<MyContentViewController, MyContentViewModel>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();
            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);

            set.Apply();

            MyCollectionTable.ReloadData();
        }

        private void AddNavigationBarItemForAddPlaylist()
        {
            var sidebarButton = new UIBarButtonItem(
            UIImage.FromFile("icon_add_static.png"),
            UIBarButtonItemStyle.Plain,
            (object sender, EventArgs e) =>
                {
                    ViewModel.CreatePlaylistCommand.Execute();
                }
            );

            NavigationItem.SetRightBarButtonItem(sidebarButton, true);
            MyCollectionTable.ReloadData();
        }
    }
}