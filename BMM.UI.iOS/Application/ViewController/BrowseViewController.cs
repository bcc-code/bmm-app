using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxTabPresentation(
        TabName = Translations.MenuViewModel_Browse,
        TabIconName = "icon_browse",
        TabSelectedIconName = "icon_browse_active",
        WrapInNavigationController = false)]
    public partial class BrowseViewController : BaseViewController<BrowseViewModel>, TTabbedViewController, IHaveLargeTitle
    {
        private bool _isRefreshing;
        private BrowseDetailsTableViewSource _source;
        private bool _isLoading;
        public double? InitialLargeTitleHeight { get; set; }

        public UIViewController[] TabViewControllers { get; private set; }

        public BrowseViewController() : base(nameof(BrowseViewController))
        { }

        public override Type ParentViewControllerType => typeof(ContainmentNavigationViewController);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var refreshControl = new MvxUIRefreshControl
            {
                TintColor = AppColors.RefreshControlTintColor
            };

            BrowseTableView.RefreshControl = refreshControl;

            _source = new BrowseDetailsTableViewSource(BrowseTableView);

            var set = this.CreateBindingSet<BrowseViewController, BrowseViewModel>();

            set.Bind(_source)
                .To(vm => vm.Documents)
                .WithConversion<DocumentListValueConverter>(ViewModel);

            set.Bind(_source)
                .For(s => s.SelectionChangedCommand)
                .To(s => s.DocumentSelectedCommand)
                .WithConversion<DocumentSelectedCommandValueConverter>();

            set.Bind(_source)
                .For(s => s.IsFullyLoaded)
                .To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();

            set.Bind(refreshControl)
                .For(r => r.IsRefreshing)
                .To(vm => vm.IsRefreshing);

            set.Bind(refreshControl)
                .For(r => r.RefreshCommand)
                .To(vm => vm.ReloadCommand);

            set.Bind(this)
                .For(v => v.IsRefreshing)
                .To(vm => vm.IsRefreshing);

            set.Apply();
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                _source.ClearOffsets();
            }
        }
    }
}