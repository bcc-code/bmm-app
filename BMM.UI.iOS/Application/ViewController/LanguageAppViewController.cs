using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class LanguageAppViewController : BaseViewController<LanguageAppViewModel>
    {
        public LanguageAppViewController()
            : base("LanguageAppViewController")
        {
        }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new BaseSimpleTableViewSource(LanguagesTableView, LanguageAppTableViewCell.Key);
            LanguagesTableView.RowHeight = 55;
            LanguagesTableView.Source = source;

            var set = this.CreateBindingSet<LanguageAppViewController, LanguageAppViewModel>();
            set.Bind(source).To(vm => vm.Languages).WithConversion(new AppLanguageListValueConverter(), ViewModel);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.LanguageSelectedCommand).WithConversion<LanguageSelectedCommandValueConverter>();
            set.Apply();

            LanguagesTableView.ReloadData();
        }
    }
}