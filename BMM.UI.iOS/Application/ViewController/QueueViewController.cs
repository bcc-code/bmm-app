using System;
using System.Linq.Expressions;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Interfaces;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class QueueViewController : BaseViewController<QueueViewModel>, IPlayerNavigationViewController
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
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.SetNavigationBarHidden(false, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetNavigationBarHidden(true, true);
        }

        protected override void SetNavigationBarAppearance() => Expression.Empty();
    }
}