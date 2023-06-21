using System;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using CoreAnimation;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Localization;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxRootPresentation(WrapInNavigationController = false)]
    public partial class UserSetupViewController : BaseViewController<UserSetupViewModel>
    {
        public UserSetupViewController()
            : base(nameof(UserSetupViewController))
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<UserSetupViewController, UserSetupViewModel>();
            set.Bind(SettingUpMessage).To(vm => vm.TextSource).WithConversion<MvxLanguageConverter>(Translations.UserSetupViewModel_SettingUpAccountMessage);
            set.Bind(LoadingSpinnerImageView).For(v => v.Hidden).To(vm => vm.IsLoading).WithConversion<InvertedVisibilityConverter>();
            set.Apply();

            AnimateSpinner();
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