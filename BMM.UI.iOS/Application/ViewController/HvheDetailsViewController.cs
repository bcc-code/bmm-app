using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views.Gestures;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class HvheDetailsViewController : BaseViewController<HvheDetailsViewModel>
    {
        private HvheDetailsTableViewSource _source;

        public HvheDetailsViewController() : base(null)
        {
        }

        public HvheDetailsViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var set = this.CreateBindingSet<HvheDetailsViewController, HvheDetailsViewModel>();

            _source = new HvheDetailsTableViewSource(ContentTableView);
            set.Bind(_source)
                .To(vm => vm.Items);
            
            set.Bind(PageTitleLabel)
                .To(vm => vm.TextSource[Translations.HvheDetailsViewModel_Title]);
            
            set.Bind(CloseIconView)
                .For(v => v.BindTap())
                .To(vm => vm.CloseCommand);
            
            set.Bind(ContentTableView.Swipe(UISwipeGestureRecognizerDirection.Right))
                .For(v => v.Command)
                .To(vm => vm.SelectLeftItemCommand);
            
            set.Bind(ContentTableView.Swipe(UISwipeGestureRecognizerDirection.Left))
                .For(v => v.Command)
                .To(vm => vm.SelectRightItemCommand);
            
            set.Apply();
            NavigationController!.NavigationBarHidden = true;
            NavigationController!.PresentationController!.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss
            };
            
            NavigationItem.Title = string.Empty;
            NavigationController.Title = string.Empty;

            SetThemes();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController!.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController!.SetNavigationBarHidden(false, true);
        }
        
        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            CloseIconView.Layer.BorderColor = AppColors.SeparatorColor.CGColor;
        }
        
        private void SetThemes()
        {
            PageTitleLabel.ApplyTextTheme(AppTheme.Title1);
            CloseIconView.Layer.BorderWidth = 0.5f;
            CloseIconView.Layer.ShadowRadius = 8;
            CloseIconView.Layer.ShadowOffset = CGSize.Empty;
            CloseIconView.Layer.ShadowOpacity = 0.1f;
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
    }
}