using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class AppIconViewController : BaseViewController<AppIconViewModel>
    {
        public AppIconViewController()
            : base(nameof(AppIconViewController))
        {
        }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new BaseSimpleTableViewSource(AppIconTableView, AppIconTableViewCell.Key);

            var set = this.CreateBindingSet<AppIconViewController, AppIconViewModel>();

            set.Bind(source)
                .For(v => v.ItemsSource)
                .To(vm => vm.AppIcons);

            set.Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.AppIconSelected);

            set.Apply();
        }
    }
}