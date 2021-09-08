using System;
using BMM.Core.ViewModels.Base;
using BMM.UI.iOS.NewMediaPlayer;
using Foundation;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public abstract class BaseViewController<TViewModel> : MvxViewController<TViewModel>, IBaseViewController where TViewModel : BaseViewModel
    {
        public abstract Type ParentViewControllerType { get; }

        public BaseViewController(string nib)
            : base(nib, null)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.TextSource[TitleKey];

            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.TextSource))
                {
                    Title = ViewModel.TextSource[TitleKey];
                }
            };

            if (VersionHelper.SupportsLargeTitles)
            {
                NavigationItem.LargeTitleDisplayMode = this is IHaveLargeTitle
                    ? UINavigationItemLargeTitleDisplayMode.Always
                    : UINavigationItemLargeTitleDisplayMode.Never;
            }
        }

        private string TitleKey => $"{ViewModel.GetType().Name}_Title";

        protected static void ClearPresentationDelegate(UIPresentationController presentationController)
        {
            if (presentationController.Delegate is CustomUIAdaptivePresentationControllerDelegate customUiAdaptivePresentationControllerDelegate)
                customUiAdaptivePresentationControllerDelegate.Clear();
        }

        public virtual void RegisterViewController(IBaseViewController viewController)
        {
            throw new Exception(this.GetType() + " has no implementation for inserting a view called " + viewController.GetType().ToString());
        }

        public bool IsVisible()
        {
            return IsViewLoaded && View.Window != null;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateOrientation();
        }

        public override void ViewSafeAreaInsetsDidChange()
        {
            base.ViewSafeAreaInsetsDidChange();
            if (this is IHaveLargeTitle largeTitleViewController)
                largeTitleViewController.InitialLargeTitleHeight ??= View?.SafeAreaInsets.Top;
        }

        /// <summary>
        /// Update the orientation of the view if the device has an orientation, not supported by the view to be displayed.
        /// </summary>
        // TODO: Check if it's worth changing the orientation back after this view disappears
        private void UpdateOrientation()
        {
            if (
                // Check if the orientation, currently on the device, is allowed for the current view (if 0, it's not allowed)
                (
                    // Exponentiate with base 2 to get a deimal number, ready for for bit-shifting
                    (int)Math.Pow(2, (int)UIDevice.CurrentDevice.Orientation) &
                    // Get the allowed orientations (binary options as integer)
                    (int)GetSupportedInterfaceOrientations()
                ) == 0
            )
            {
                UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)PreferredInterfaceOrientationForPresentation()), new NSString("orientation"));
            }
        }
    }
}