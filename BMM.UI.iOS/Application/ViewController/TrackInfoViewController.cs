using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class TrackInfoViewController : BaseViewController<TrackInfoViewModel>
    {
        public TrackInfoViewController() : base(nameof(TrackInfoViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<TrackInfoViewController, TrackInfoViewModel>();
            var source = new TrackInfoTableViewSource(TrackInfoTableView);

            set
                .Bind(source)
                .For(s => s.ItemsSource)
                .To(po => po.Items);

            set
                .Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(vm => vm.ItemSelectedCommand);
            
            set.Apply();
        }
    }
}