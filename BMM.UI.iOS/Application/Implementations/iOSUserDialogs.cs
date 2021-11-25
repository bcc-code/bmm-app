using System;
using System.Security.Cryptography.X509Certificates;
using Acr.UserDialogs;
using BMM.Core.Implementations;
using BMM.Core.ViewModels;
using TTG;
using UIKit;

namespace BMM.UI.iOS
{
    public class iOSUserDialogs : UserDialogsImpl
    {
        private readonly IViewModelAwareViewPresenter _viewModelAwareViewPresenter;
        private int DefaultSnackbarMargin = 8;
        private IDisposable _currentToast;

        public iOSUserDialogs(IViewModelAwareViewPresenter viewModelAwareViewPresenter)
        {
            _viewModelAwareViewPresenter = viewModelAwareViewPresenter;
        }

        public override IDisposable Toast(ToastConfig cfg)
        {
            _currentToast?.Dispose();

            var app = UIApplication.SharedApplication;
            app.SafeInvokeOnMainThread(() =>
            {
                var snackbar = new TTGSnackbar
                {
                    Message = cfg.Message,
                    Duration = cfg.Duration,
                    AnimationType = TTGSnackbarAnimationType.FadeInFadeOut,
                    ShowOnTop = cfg.Position == ToastPosition.Top,
                    BottomMargin = GetBottomMarign(cfg.Position)
                };

                if (cfg.Icon != null)
                    snackbar.Icon = UIImage.FromBundle(cfg.Icon);

                if (cfg.BackgroundColor != null)
                    snackbar.BackgroundColor = cfg.BackgroundColor.Value.ToNative();

                if (cfg.MessageTextColor != null)
                    snackbar.MessageLabel.TextColor = cfg.MessageTextColor.Value.ToNative();

                if (cfg.Action != null)
                {
                    var color = cfg.Action.TextColor ?? ToastConfig.DefaultActionTextColor;
                    if (color != null)
                        snackbar.ActionButton.SetTitleColor(color.Value.ToNative(), UIControlState.Normal);

                    snackbar.ActionText = cfg.Action.Text;
                    snackbar.ActionBlock = x =>
                    {
                        snackbar.Dismiss();
                        cfg.Action.Action?.Invoke();
                    };
                }
                snackbar.Show();

                _currentToast = new DisposableAction(
                    () => app.SafeInvokeOnMainThread(() => snackbar.Dismiss())
                );
            });

            return _currentToast;
        }

        private nfloat GetBottomMarign(ToastPosition? toastPosition)
        {
            if (toastPosition == ToastPosition.Top)
                return 0f;

            nfloat playerHeight = MiniPlayerViewController.MiniPlayerHeight;
            nfloat bottomBarSpace = MenuViewController.BottomBarHeight - UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom;

            var playerNavigationController = ((Presenter)_viewModelAwareViewPresenter).CurrentRootViewController as PlayerNavigationController;

            if (playerNavigationController != null)
            {
                playerHeight = 0f;

                if (!(playerNavigationController.TopViewController is PlayerViewController))
                    bottomBarSpace = 0f;
            }

            return playerHeight + bottomBarSpace + DefaultSnackbarMargin;
        }
    }
}