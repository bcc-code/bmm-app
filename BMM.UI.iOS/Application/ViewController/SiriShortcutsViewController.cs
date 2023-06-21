using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class SiriShortcutsViewController : BaseViewController<SiriShortcutsViewModel>
    {
        public SiriShortcutsViewController()
            : base(nameof(SiriShortcutsViewController))
        {
            CurrentInstance = this;
        }

        public static SiriShortcutsViewController CurrentInstance { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var source = new BaseSimpleTableViewSource(ThemeSettingsTableView, SelectThemeTableViewCell.Key);

            var set = this.CreateBindingSet<SiriShortcutsViewController, SiriShortcutsViewModel>();

            set.Bind(source)
                .For(v => v.ItemsSource)
                .To(vm => vm.AvailableShortcuts);

            set.Bind(source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.ShortcutSelectedCommand);

            set.Apply();
        }
    }
}