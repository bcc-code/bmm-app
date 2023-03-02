using System;
using BMM.Core.Helpers;
using BMM.Core.ViewModels.Base;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace BMM.UI.iOS
{
    public static class TTabbedViewControllerMethods
    {
        public static UIViewController[] InitializeTabs(this TTabbedViewController client, UISegmentedControl segmentedControl, UIView contentView, BaseViewModel[] viewModels, int defaultTab)
        {
            segmentedControl.ApportionsSegmentWidthsByContent = false;
            // Remove the default tabs of the control
            segmentedControl.RemoveAllSegments();

            // Initialize the tabs
            UIViewController[] viewControllers = new UIViewController[viewModels.Length];
            for (int key = 0; key < viewModels.Length; ++key)
                viewControllers[key] = client.AddTabByViewModel(segmentedControl, viewModels[key], key);

            // Update the active view as the selection in the TabSegmentControl changes
            segmentedControl.ValueChanged += (object sender, EventArgs e) =>
            {
                SetActiveView(contentView, viewControllers[(int)segmentedControl.SelectedSegment]);
            };

            // Set the default tab
            segmentedControl.SelectedSegment = new IntPtr(defaultTab);
            SetActiveView(contentView, viewControllers[defaultTab]);

            return viewControllers;
        }

        private static UIViewController AddTabByViewModel(this TTabbedViewController client, UISegmentedControl segmentedControll, BaseViewModel vm, int index)
        {
            segmentedControll.InsertSegment(
                vm.TextSource.GetText(ViewModelUtils.GetVMTitleKey(vm.GetType())),
                new IntPtr(index),
                false
            );

            return (client as IMvxCanCreateIosView).CreateViewControllerFor(vm) as UIViewController;
        }

        private static void SetActiveView(UIView contentView, UIViewController viewController)
        {
            if (contentView.Subviews.Length != 0)
            {
                foreach (var view in contentView.Subviews)
                    view.RemoveFromSuperview();
            }

            contentView.AddSubview(viewController.View);

            // Update the boundaries and the positioning of the element, to fit the one, the contentView has.
            viewController.View.Bounds = contentView.Bounds;
            viewController.View.Center = new CoreGraphics.CGPoint(contentView.Bounds.Width / 2, contentView.Bounds.Height / 2);
        }
    }
}