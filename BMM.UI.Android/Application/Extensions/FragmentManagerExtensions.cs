using AndroidX.Fragment.App;
using BMM.UI.Droid.Application.Helpers;
using Java.Lang;
using MvvmCross.Platforms.Android.Views;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class FragmentManagerExtensions
    {
        public static Fragment Instantiate(this FragmentManager fragmentManager, string fragmentName)
            => fragmentManager
                .FragmentFactory
                .Instantiate(
                    ClassLoader.SystemClassLoader,
                    fragmentName);

        public static Fragment Instantiate(this FragmentManager fragmentManager, BmmViewPagerFragmentInfo fragmentInfo)
            => fragmentManager
                .FragmentFactory
                .Instantiate(
                    ClassLoader.SystemClassLoader,
                    fragmentInfo.FragmentType.FragmentJavaName());
    }
}