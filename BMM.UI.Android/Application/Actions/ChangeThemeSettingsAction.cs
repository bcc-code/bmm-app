using System.Threading.Tasks;
using AndroidX.AppCompat.App;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Theme.Interfaces;
using BMM.Core.Implementations;
using BMM.Core.Models.Themes;
using BMM.UI.Droid.Utils;
using MvvmCross.Base;
using MvvmCross.Navigation;

namespace BMM.UI.Droid.Application.Actions
{
    public class ChangeThemeSettingsAction
        : GuardedActionWithParameter<Theme>,
          IChangeThemeSettingsAction
    {
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;

        public ChangeThemeSettingsAction(IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher)
        {
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
        }

        protected override Task Execute(Theme theme)
        {
            _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                AppCompatDelegate.DefaultNightMode = ThemeUtils.GetUIModeForTheme(theme);
            });

            return Task.CompletedTask;
        }
    }
}