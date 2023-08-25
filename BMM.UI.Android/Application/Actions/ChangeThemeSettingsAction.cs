using AndroidX.AppCompat.App;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Theme.Interfaces;
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.Themes;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Utils;
using MvvmCross.Base;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Actions
{
    public class ChangeThemeSettingsAction
        : GuardedActionWithParameter<Theme>,
          IChangeThemeSettingsAction
    {
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IMvxAndroidCurrentTopActivity _mvxAndroidCurrentTopActivity;

        public ChangeThemeSettingsAction(
            IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher,
            IMediaPlayer mediaPlayer,
            IMvxAndroidCurrentTopActivity mvxAndroidCurrentTopActivity)
        {
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
            _mediaPlayer = mediaPlayer;
            _mvxAndroidCurrentTopActivity = mvxAndroidCurrentTopActivity;
        }

        protected override Task Execute(Theme theme)
        {
            _mediaPlayer.Pause();
            _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                AppCompatDelegate.DefaultNightMode = ThemeUtils.GetUIModeForTheme(theme);
                _mvxAndroidCurrentTopActivity.Activity.Recreate();
            });

            return Task.CompletedTask;
        }
    }
}