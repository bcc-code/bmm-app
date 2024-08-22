using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using CoreAnimation;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxRootPresentation(WrapInNavigationController = false)]
    public partial class OidcLoginViewController : BaseViewController<OidcLoginViewModel>
    {
        // The login is done by replacing the RootViewController by the LoginViewController. It replaces all, that's available on the current screen.
        public override Type ParentViewControllerType => null;

        public OidcLoginViewController()
            : base(nameof(OidcLoginViewController))
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            LoginButton.TouchUpInside += delegate { View.EndEditing(true); };

            var set = this.CreateBindingSet<OidcLoginViewController, OidcLoginViewModel>();
            set.Bind(SubtitleLabel).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.LoginViewModel_LoginInfo);
            set.Bind(SubtitleLabel).For(v => v.Hidden).To(vm => vm.IsLoading);
            set.Bind(LoadingSpinnerImageView).For(v => v.Hidden).To(vm => vm.IsLoading).WithConversion<InvertedBoolConverter>();
            set.Bind(LoginButton).For(v => v.Hidden).To(vm => vm.IsLoading);
            set.Bind(LoginButton).To(vm => vm.LoginCommand);
            set.Bind(LoginButton).For(v => v.BindTitle()).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.LoginViewModel_BtnLogin_Text);
            set.Apply();

            AnimateSpinner();
            if (ViewModel.IsInitialLogin)
            {
                ViewModel.LoginCommand.Execute();
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
        }

        public void AnimateSpinner()
        {
            CABasicAnimation rotationAnimation = CABasicAnimation.FromKeyPath("transform.rotation");
            rotationAnimation.To = NSNumber.FromDouble(Math.PI * 2); // full rotation (in radians)
            rotationAnimation.RepeatCount = int.MaxValue; // repeat forever
            rotationAnimation.Duration = 1;

            // Give the added animation a key for referencing it later (to remove, in this case).
            LoadingSpinnerImageView.Layer.AddAnimation(rotationAnimation, "rotationAnimation");
        }
    }
}