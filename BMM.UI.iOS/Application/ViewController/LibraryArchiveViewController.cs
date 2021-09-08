using System;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.iOS
{
    public partial class LibraryArchiveViewController : BaseViewController<LibraryArchiveViewModel>
    {
        public LibraryArchiveViewController() : base(nameof(LibraryArchiveViewController))
        {
        }

        public override Type ParentViewControllerType => typeof(LibraryViewController);

        public override void ViewDidLoad()
        {
            Title = "Archive";

            base.ViewDidLoad();

            var source = new ArchiveTableViewSource(ArchiveTableView);

            var set = this.CreateBindingSet<LibraryArchiveViewController, LibraryArchiveViewModel>();
            set.Bind(source).For(s => s.ItemsSource).To(vm => vm.ListItems);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(vm => vm.ItemSelectedCommand);
            set.Apply();

            ArchiveTableView.ReloadData();
        }
    }
}

