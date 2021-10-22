using Android.Runtime;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.Listeners;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.BrowseFragment")]
    public class BrowseFragment : SwipeToRefreshFragment<BrowseViewModel>
    {
        private BrowseRecyclerAdapter _browseRecyclerAdapter;

        private BrowseRecyclerAdapter BrowseRecyclerAdapter =>
            _browseRecyclerAdapter ??= new BrowseRecyclerAdapter((IMvxAndroidBindingContext) BindingContext);

        protected override void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            base.InitRecyclerView(recyclerView);
            recyclerView.SetItemViewCacheSize(20);
            recyclerView!.Adapter = BrowseRecyclerAdapter;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            RecyclerView.AddOnScrollListener(new ImageServiceRecyclerViewScrollListener());
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            RecyclerView.ClearOnScrollListeners();
        }

        protected override int FragmentId => Resource.Layout.fragment_browse;
    }
}