using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using System.Collections.Generic;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.Tabs;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.ViewPager;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.LibraryFragment")]
    public class LibraryFragment : BaseFragment<LibraryViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            var viewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);
            if (viewPager != null)
            {
                var fragments = new List<MvxViewPagerFragmentInfo>();
                if (ViewModel.ViewModelPodcasts != null)
                {
                    fragments.Add(new MvxViewPagerFragmentInfo(
                        ViewModel.ViewModelPodcasts.TextSource.GetText("Title"),
                        "Podcasts",
                        typeof(PodcastsFragment),
                        MvxViewModelRequest.GetDefaultRequest(typeof(PodcastsViewModel))));
                    fragments.Add(new MvxViewPagerFragmentInfo(
                        ViewModel.ViewModelArchive.TextSource.GetText("Title"),
                        "LibraryArchive",
                        typeof(LibraryArchiveFragment),
                        MvxViewModelRequest.GetDefaultRequest(typeof(LibraryArchiveViewModel))));
                }
                viewPager.Adapter = new MvxCachingFragmentStatePagerAdapter(Activity, ChildFragmentManager, fragments);
                viewPager.SetCurrentItem((int)ViewModel.TabAtOpen, false);
                view.FindViewById<TabLayout>(Resource.Id.tabs)?.SetupWithViewPager(viewPager);
            }
            return view;
        }
        protected override int FragmentId => Resource.Layout.fragment_library;
    }
}