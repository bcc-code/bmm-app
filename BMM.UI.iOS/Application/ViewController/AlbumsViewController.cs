using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;

namespace BMM.UI.iOS
{
    public partial class AlbumsViewController : BaseViewController<AlbumsViewModel>
    {
        public AlbumsViewController()
            : base("AlbumsViewController")
        {
        }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl {TintColor = AppColors.RefreshControlTintColor};
            AlbumsTable.RefreshControl = refreshControl;

            var source = new DocumentsTableViewSource(AlbumsTable);

            var set = this.CreateBindingSet<AlbumsViewController, AlbumsViewModel>();
            set.Bind(this).For(v => v.Title).To(vm => vm.Year);
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsRefreshing);
            set.Bind(refreshControl).For(r => r.RefreshCommand).To(vm => vm.ReloadCommand);
            set.Apply();

            AlbumsTable.ReloadData();
        }
    }
}