namespace BMM.UI.iOS
{
    public delegate void DidDismissDelegate(UIPresentationController presentationController);
    public delegate void DidAttemptToDismiss(UIPresentationController presentationController);
    public delegate bool ShouldDismiss(UIPresentationController presentationController);

    public class CustomUIAdaptivePresentationControllerDelegate : UIAdaptivePresentationControllerDelegate
    {
        public DidDismissDelegate OnDidDismiss { get; set; }
        public DidAttemptToDismiss OnDidAttemptToDismiss { get; set; }
        public ShouldDismiss OnShouldDismiss { get; set; }

        public override void DidDismiss(UIPresentationController presentationController)
            => OnDidDismiss?.Invoke(presentationController);

        public override void DidAttemptToDismiss(UIPresentationController presentationController)
            => OnDidAttemptToDismiss?.Invoke(presentationController);

        public override bool ShouldDismiss(UIPresentationController presentationController)
            => OnShouldDismiss?.Invoke(presentationController) ?? true;

        public void Clear()
        {
            OnDidDismiss = null;
            OnDidAttemptToDismiss = null;
            OnShouldDismiss = null;
        }
    }
}