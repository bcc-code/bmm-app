using BMM.Core.Translation;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(TabName = Translations.MenuViewModel_Profile, TabIconName = "icon_profile", TabSelectedIconName = "icon_profile_active", WrapInNavigationController = false)]
    public partial class SettingsViewController : BaseViewController<SettingsViewModel>, IHaveLargeTitle
    {
        public double? InitialLargeTitleHeight { get; set; }

        public SettingsViewController()
            : base(nameof(SettingsViewController))
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new SettingsTableViewSource(SettingsTableView);

            var set = this.CreateBindingSet<SettingsViewController, SettingsViewModel>();
            set.Bind(source).To(vm => vm.ListItems);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.ItemSelectedCommand);
            set.Bind(FormattedText).For(s => s.StyledTextContainer).To(vm => vm.StyledTextContainer);
            set.Apply();
        }
    }
}