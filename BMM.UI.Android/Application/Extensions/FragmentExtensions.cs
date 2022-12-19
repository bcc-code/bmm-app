using System;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Extensions
{
    public static class FragmentExtensions
    {
        public static BmmViewPagerFragmentInfo CreateFragmentForViewPager(
            this MvxFragment fragment,
            string title,
            string tag,
            Type fragmentType,
            IMvxViewModel vm)
            => new BmmViewPagerFragmentInfo(
                title,
                tag,
                fragmentType,
                new MvxViewModelInstanceRequest(vm));
    }
}
