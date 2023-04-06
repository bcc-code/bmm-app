using Acr.UserDialogs;
using BMM.Core.Constants;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.Parameters;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews;
using MvvmCross.Commands;

namespace BMM.UI.iOS.Implementations.UI
{
    public class iOSDialogService : IDialogService
    {
        private void CloseDialogs()
        {
            var dialogs = KeyWindow.Subviews.OfType<BmmDialog>();
            foreach (var dialog in dialogs)
                dialog.RemoveFromSuperview();
        }

        private static UIWindow KeyWindow => UIApplication.SharedApplication.KeyWindow;

        public async Task ShowDialog(IDialogParameter dialog)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(
                () =>
                {
                    var view = CreateDialogView(dialog);
                    KeyWindow.Add(view);

                    UIView.Animate(
                        ViewConstants.DefaultAnimationDuration,
                        () => view.Alpha = ViewConstants.VisibleAlpha);
                });
        }

        private BmmDialog CreateDialogView(IDialogParameter dialogParameter)
        {
            var view = new BmmDialog(UIApplication.SharedApplication.KeyWindow.Bounds);
            view.DataContext = dialogParameter;
            view.Layer.ZPosition = 1000;
            view.Alpha = ViewConstants.InvisibleAlpha;
            return view;
        }
    }
}