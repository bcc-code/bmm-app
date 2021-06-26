using System;
using BMM.Core.ViewModels;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    [Register(nameof(ShareTrackCollectionViewController))]
    public class ShareTrackCollectionViewController : BaseViewController<ShareTrackCollectionViewModel>
    {
        public ShareTrackCollectionViewController() : base(null)
        {
        }

        public ShareTrackCollectionViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate()
            {
                OnDidDismiss = OnDidDismiss,
                OnDidAttemptToDismiss = OnDidAttemptToDismiss
            };

            PrepareHeader();
        }

        private void PrepareHeader()
        {
            var saveButton = new UIBarButtonItem(
                ViewModel.TextSource.GetText("Done"),
                UIBarButtonItemStyle.Plain,
                (sender, e) => { ViewModel.CloseCommand.Execute(); }
            );

            NavigationItem.SetRightBarButtonItem(saveButton, true);
        }

        private void OnDidAttemptToDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
        }

        private void OnDidDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();

            if (presentationController.Delegate is CustomUIAdaptivePresentationControllerDelegate customUiAdaptivePresentationControllerDelegate)
                customUiAdaptivePresentationControllerDelegate.Clear();
        }
    }
}