using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class QueueViewController : BaseViewController<QueueViewModel>
    {
        public QueueViewController()
            : base(nameof(QueueViewController))
        {
        }

        public override System.Type ParentViewControllerType => typeof(UINavigationController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new QueueTableViewSource(QueueTableView);

            var set = this.CreateBindingSet<QueueViewController, QueueViewModel>();
            set.Bind(source).To(vm => vm.Documents).WithConversion<DocumentListValueConverter>(ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.DocumentSelectedCommand).WithConversion<DocumentSelectedCommandValueConverter>();
            set.Apply();

            QueueTableView.ReloadData();
        }
    }
}