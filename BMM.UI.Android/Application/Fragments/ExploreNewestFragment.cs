using Android.Runtime;
using Android.Views;
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
    [Register("bmm.ui.droid.application.fragments.ExploreNewestFragment")]
    public class ExploreNewestFragment : SwipeToRefreshFragment<ExploreNewestViewModel>
    {
        private PodcastContextHeaderRecyclerAdapter _podcastContextHeaderRecyclerAdapter;

        private PodcastContextHeaderRecyclerAdapter PodcastContextHeaderRecyclerAdapter =>
            _podcastContextHeaderRecyclerAdapter ??= new PodcastContextHeaderRecyclerAdapter((IMvxAndroidBindingContext) BindingContext, ViewModel);

        protected override void InitRecyclerView(MvxRecyclerView recyclerView)
        {
            base.InitRecyclerView(recyclerView);
            recyclerView.SetItemViewCacheSize(20);
            recyclerView!.Adapter = PodcastContextHeaderRecyclerAdapter;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.playback_history, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.playback_history)
                ViewModel.NavigateToViewModelCommand.Execute(typeof(PlaybackHistoryViewModel));

            return base.OnOptionsItemSelected(item);
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

        protected override int FragmentId => Resource.Layout.fragment_explore;
    }
}