using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class LanguageContentViewController : BaseViewController<LanguageContentViewModel>
    {
        public LanguageContentViewController()
            : base(nameof(LanguageContentViewController))
        {
        }

        public override System.Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new LanguageEditableTableViewSource(LanguagesTableView);

            var set = this.CreateBindingSet<LanguageContentViewController, LanguageContentViewModel>();
            set.Bind(source).To(vm => vm.Languages);
            set.Bind(source).For(s => s.IsFullyLoaded).To(vm => vm.IsLoading).WithConversion<InvertedBoolConverter>();
            set.Apply();

            LanguagesTableView.ReloadData();

            AddNavigationItems();
        }

        public void AddNavigationItems()
        {
            UIBarButtonItem edit = null;
            UIBarButtonItem oldLeftButton = null;

            var add = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) =>
            {
                ViewModel.OpenSelectLanguageDialogCommand.Execute();
            });
            var done = new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) =>
            {
                LanguagesTableView.SetEditing(false, true);
                NavigationItem.LeftBarButtonItem = oldLeftButton;
                NavigationItem.RightBarButtonItem = edit;
                oldLeftButton = null;

            });
            edit = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (s, e) =>
            {
                if (LanguagesTableView.Editing)
                    LanguagesTableView.SetEditing(false, true); // if we've half-swiped a row

                LanguagesTableView.SetEditing(true, true);
                oldLeftButton = NavigationItem.LeftBarButtonItem;
                NavigationItem.LeftBarButtonItem = add;
                NavigationItem.RightBarButtonItem = done;

            });

            NavigationItem.RightBarButtonItem = edit;
        }
    }
}