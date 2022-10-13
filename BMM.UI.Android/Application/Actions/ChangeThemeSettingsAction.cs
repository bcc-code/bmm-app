using System.Threading.Tasks;
using AndroidX.AppCompat.App;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Theme.Interfaces;
using BMM.Core.Implementations;
using BMM.Core.Models.Themes;
using BMM.Core.NewMediaPlayer.Abstractions;
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
        private readonly IMediaPlayer _mediaPlayer;

        public ChangeThemeSettingsAction(
            IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher,
            IMediaPlayer mediaPlayer)
        {
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
            _mediaPlayer = mediaPlayer;
        }

        protected override Task Execute(Theme theme)
        {
            _mediaPlayer.Pause();
            _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                AppCompatDelegate.DefaultNightMode = ThemeUtils.GetUIModeForTheme(theme);
            });

            return Task.CompletedTask;
        }
    }
}