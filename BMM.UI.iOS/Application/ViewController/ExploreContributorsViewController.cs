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
            set.Bind(source).To(vm => vm.Documents);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand);
            set.Apply();

            TrackTableView.ReloadData();
        }
    }
}