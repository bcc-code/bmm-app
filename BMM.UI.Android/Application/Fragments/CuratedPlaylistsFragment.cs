using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.CuratedPlaylists")]
    public class CuratedPlaylists : BaseFragment<CuratedPlaylistsViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_curatedplaylists;

        protected override void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            if (recyclerView == null)
                return;

            recyclerView.HasFixedSize = true;
            var layoutManager = new GridLayoutManager(ParentActivity, 2);
            recyclerView.SetLayoutManager(layoutManager);
        }
    }
}