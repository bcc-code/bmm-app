using System;
using System.Linq.Expressions;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Interfaces;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace BMM.UI.iOS
{
    public partial class ChangeTrackLanguageViewController : BaseViewController<ChangeTrackLanguageViewModel>, IPlayerNavigationViewController
    {
        public ChangeTrackLanguageViewController() : base(nameof(ChangeTrackLanguageViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new ChangeTrackLanguageTableViewSource(ChangeTrackLanguageTableView);

            var set = this.CreateBindingSet<ChangeTrackLanguageViewController, ChangeTrackLanguageViewModel>();
            set.Bind(source).To(vm => vm.AvailableLanguages);
            set.Bind(source).For(s => s.SelectionChangedCommand).To(s => s.LanguageSelectedCommand);

            set.Apply();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.SetNavigationBarHidden(false, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetNavigationBarHidden(true, true);
        }

        protected override void SetNavigationBarAppearance() => Expression.Empty();
    }
}