using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class ThemeSettingsViewController : BaseViewController<ThemeSettingsViewModel>
    {
        public ThemeSettingsViewController()
            : base(nameof(ThemeSettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new BaseSimpleTableViewSource(ThemeSettingsTableView, SelectThemeTableViewCell.Key);

            var set = this.CreateBindingSet<ThemeSettingsViewController, ThemeSettingsViewModel>();

            set.Bind(source)
                .For(v => v.ItemsSource)
                .To(vm => vm.ThemeSettings);

            set.Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.ThemeSelectedCommand);

            set.Apply();
        }
    }
}