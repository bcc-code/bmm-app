using System;
using Android.Content;
using Android.Graphics;
using AndroidX.Browser.CustomTabs;
using BMM.Core.Implementations.UI;
using MvvmCross.Platforms.Android;

namespace BMM.UI.Droid.Application.Implementations.UI
{
    public class UriOpener: IUriOpener
    {
        private readonly IMvxAndroidCurrentTopActivity _topActivity;

        public UriOpener(IMvxAndroidCurrentTopActivity topActivity)
        {
            _topActivity = topActivity;
        }

        public void OpenUri(Uri uri)
        {
            var intentBuilder = new CustomTabsIntent.Builder()
                .SetToolbarColor(Color.Argb(255, 52, 152, 219))
                .SetShowTitle(true);

            var customTabsIntent = intentBuilder.Build();

            var mgr = new CustomTabsActivityManager(_topActivity.Activity);

            mgr.LaunchUrl(uri.AbsoluteUri, customTabsIntent);

            if (!mgr.BindService())
            {
                var defaultIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(uri.ToString()));
                var context = Android.App.Application.Context;
                defaultIntent.SetFlags(ActivityFlags.NewTask);
                context.StartActivity(defaultIntent);
            }
        }
    }
}