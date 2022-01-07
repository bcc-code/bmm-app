using System.Threading.Tasks;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Theme.Interfaces;
using BMM.Core.Models.Themes;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.Utils;
using MvvmCross.Base;
using UIKit;

namespace BMM.UI.iOS.Actions
{
    public class ChangeThemeSettingsAction : GuardedActionWithParameter<Theme>, IChangeThemeSettingsAction
    {
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;

        public ChangeThemeSettingsAction(IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
        {
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
        }

        protected override async Task Execute(Theme theme)
        {
            await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                UIView.Animate(
                    ViewConstants.DefaultAnimationDuration,
                    () =>
                    {
                        UIApplication.SharedApplication.KeyWindow.OverrideUserInterfaceStyle = ThemeUtils.GetUIUserInterfaceStyleForTheme(theme);

                        if (theme == Theme.Dark)
                            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
                        else
                            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, true);
                    });
            });
        }
    }
}