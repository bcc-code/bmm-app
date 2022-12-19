using MvvmCross.Platforms.Ios.Views;

namespace BMM.UI.iOS.Delegates
{
    public delegate MvxViewController CreateOrRefreshViewControllerDelegate(
        object item, 
        MvxViewController existingViewController = null);
}