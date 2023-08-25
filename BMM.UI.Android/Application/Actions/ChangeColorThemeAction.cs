using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Theme.Interfaces;
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.Themes;
using BMM.Core.NewMediaPlayer.Abstractions;
using MvvmCross.Base;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Actions
{
    public class ChangeColorThemeAction
        : GuardedActionWithParameter<ColorTheme>,
          IChangeColorThemeAction
    {
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IMvxAndroidCurrentTopActivity _mvxAndroidCurrentTopActivity;

        public ChangeColorThemeAction(
            IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher,
            IMediaPlayer mediaPlayer,
            IMvxAndroidCurrentTopActivity mvxAndroidCurrentTopActivity)
        {
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
            _mediaPlayer = mediaPlayer;
            _mvxAndroidCurrentTopActivity = mvxAndroidCurrentTopActivity;
        }

        protected override Task Execute(ColorTheme parameter)
        {
            AppSettings.SelectedColorTheme = parameter;

            _mediaPlayer.Pause();
            _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                _mvxAndroidCurrentTopActivity.Activity.Recreate();
            });

            return Task.CompletedTask;
        }
    }
}