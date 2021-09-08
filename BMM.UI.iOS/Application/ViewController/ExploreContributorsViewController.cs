using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class ExploreContributorsViewController : BaseViewController<ExploreContributorsViewModel>
    {
        public ExploreContributorsViewController()
            : base(nameof(ExploreContributorsViewController))
        {
        }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new DocumentsTableViewSource(TrackTableView);

            var set = this.CreateBindingSet<ExploreContributorsViewController, ExploreContributorsViewModel>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Bind(source).For(s => s.LoadMoreCommand).To(s => s.LoadMoreCommand);
            set.Bind(source).For(s => s.IsFullyLoaded).To(s => s.IsFullyLoaded);
            set.Apply();

            TrackTableView.ReloadData();
        }
    }
}