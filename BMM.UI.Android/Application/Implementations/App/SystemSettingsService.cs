using System.Threading.Tasks;
using Android.Content;
using MvvmCross.Base;
using MvvmCross.Platforms.Android;
using Xamarin.Essentials;

namespace BMM.UI.Droid.Application.Implementations.App
{
    public class SystemSettingsService : ISystemSettingsService
    {
        private readonly IMvxMainThreadAsyncDispatcher _mvxMainThreadAsyncDispatcher;
        private readonly IMvxAndroidCurrentTopActivity _mvxAndroidCurrentTopActivity;

        public SystemSettingsService(
            IMvxMainThreadAsyncDispatcher mvxMainThreadAsyncDispatcher,
            IMvxAndroidCurrentTopActivity mvxAndroidCurrentTopActivity)
        {
            _mvxMainThreadAsyncDispatcher = mvxMainThreadAsyncDispatcher;
            _mvxAndroidCurrentTopActivity = mvxAndroidCurrentTopActivity;
        }
        
        public async Task GoToSettings()
        {
            await _mvxMainThreadAsyncDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                var context = _mvxAndroidCurrentTopActivity.Activity;

                var settingsIntent = new Intent();
                settingsIntent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
                settingsIntent.AddCategory(Intent.CategoryDefault);
                settingsIntent.SetData(Android.Net.Uri.Parse("package:" + Platform.AppContext.PackageName));

                var flags = ActivityFlags.NewTask | ActivityFlags.NoHistory | ActivityFlags.ExcludeFromRecents;
                settingsIntent.SetFlags(flags);

                context.StartActivity(settingsIntent);
            });
        }
    }
}