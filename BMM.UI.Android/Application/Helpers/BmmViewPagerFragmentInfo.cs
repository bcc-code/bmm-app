using System;
using MvvmCross.Platforms.Android.Views.ViewPager;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Helpers
{
    public class BmmViewPagerFragmentInfo : MvxViewPagerFragmentInfo
    {
        public BmmViewPagerFragmentInfo(
            string title,
            string tag,
            Type fragmentType,
            MvxViewModelInstanceRequest request)
            : base(title, tag, fragmentType, request)
        {
        }

        public IMvxViewModel ViewModel => (Request as MvxViewModelInstanceRequest)?.ViewModelInstance;
    }
}