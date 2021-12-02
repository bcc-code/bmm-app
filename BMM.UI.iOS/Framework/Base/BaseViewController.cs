using System;
using System.ComponentModel;
using BMM.Core.Helpers;
using BMM.Core.ViewModels.Base;
using BMM.UI.iOS.Constants;
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
            SubscribeToTextSourceChange();

            SetupLargeTitle();
            SetNavigationBarAppearance();
        }

        protected virtual void SetNavigationBarAppearance()
        {
            if (NavigationController?.NavigationBar == null || !UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                return;

            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.BackgroundColor = AppColors.BackgroundPrimaryColor;
            appearance.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = AppColors.LabelPrimaryColor
            };
            appearance.ShadowColor = UIColor.Clear;
            NavigationController.NavigationBar.StandardAppearance = appearance;
            NavigationController.NavigationBar.ScrollEdgeAppearance = appearance;
        }

        private void SetupLargeTitle()
        {
            if (!VersionHelper.SupportsLargeTitles)
                return;

            if (this is IHaveLargeTitle)
            {
                NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Always;
                NavigationController.NavigationBar.PrefersLargeTitles = true;
                NavigationController.NavigationBar.SizeToFit();
            }
            else
            {
                NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Never;
            }
        }

        private string TitleKey => ViewModelUtils.GetVMTitleKey(ViewModel.GetType());

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

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AttachEvents();
            UpdateOrientation();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            DetachEvents();
        }

        protected virtual void AttachEvents()
        {
        }

        protected virtual void DetachEvents()
        {
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);
            if (parent == null)
                UnsubscribeFromTextSourceChange();
        }

        private void SubscribeToTextSourceChange()
        {
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void UnsubscribeFromTextSourceChange()
        {
            ViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.TextSource))
                Title = ViewModel.TextSource[TitleKey];
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