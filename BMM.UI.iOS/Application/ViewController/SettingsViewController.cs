using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(TabName = "Profile", TabIconName = "icon_profile", TabSelectedIconName = "icon_profile_active", WrapInNavigationController = false)]
    public partial class SettingsViewController : BaseViewController<SettingsViewModel>, IHaveLargeTitle
    {
        public double? InitialLargeTitleHeight { get; set; }

        public SettingsViewController()
            : base("SettingsViewController")
        { }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            Title = "Settings";

            base.ViewDidLoad();

            var source = new SettingsTableViewSource(SettingsTableView);

            var set = this.CreateBindingSet<SettingsViewController, SettingsViewModel>();
            set.Bind(source).To(vm => vm.ListItems);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.ItemSelectedCommand);
            set.Apply();
        }

    }
}