using System;
using Android.Content;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.UI;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Implementations.UI
{
    public class UriOpener : BaseUriOpener
    {
        private readonly IMvxAndroidCurrentTopActivity _topActivity;

        public UriOpener(IMvxAndroidCurrentTopActivity topActivity, IAnalytics analytics) : base(analytics)
        {
            _topActivity = topActivity;
        }

        protected override void PlatformOpenUri(Uri uri)
        {
            var defaultIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(uri.ToString()));
            var context = Android.App.Application.Context;
            defaultIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(defaultIntent);
        }
    }
}