using UIKit;

namespace BMM.UI.iOS
{
    public delegate void DidDismissDelegate(UIPresentationController presentationController);
    public delegate void DidAttemptToDismiss(UIPresentationController presentationController);

    public class CustomUIAdaptivePresentationControllerDelegate : UIAdaptivePresentationControllerDelegate
    {
        public DidDismissDelegate OnDidDismiss { get; set; }
        public DidAttemptToDismiss OnDidAttemptToDismiss { get; set; }

        public override void DidDismiss(UIPresentationController presentationController)
            => OnDidDismiss?.Invoke(presentationController);

        public override void DidAttemptToDismiss(UIPresentationController presentationController)
            => OnDidAttemptToDismiss?.Invoke(presentationController);

        public void Clear()
        {
            OnDidDismiss = null;
            OnDidAttemptToDismiss = null;
        }
    }
}