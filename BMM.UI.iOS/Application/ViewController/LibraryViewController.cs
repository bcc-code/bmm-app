using System;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Base;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(TabName = Translations.MenuViewModel_Library, TabIconName = "icon_library", TabSelectedIconName = "icon_library_active", WrapInNavigationController = false)]
    public partial class LibraryViewController : BaseViewController<BrowseViewModel>, TTabbedViewController, IHaveLargeTitle
    {
        public double? InitialLargeTitleHeight { get; set; }

        public UIViewController[] TabViewControllers { get; private set; }

        public LibraryViewController() : base(nameof(LibraryViewController))
        { }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TabViewControllers = this.InitializeTabs(
                TabsSegmentedControl,
                ContentView,
                new BaseViewModel[] {ViewModel.ViewModelPodcasts, ViewModel.ViewModelArchive},
                (int)ViewModel.TabAtOpen
            );
            ViewModel.ViewModelPodcasts.Initialize();
            ViewModel.ViewModelArchive.Initialize();
        }
    }
}