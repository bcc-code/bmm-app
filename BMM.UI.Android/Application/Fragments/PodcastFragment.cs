using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.PodcastFragment")]
    public class PodcastFragment : BaseFragment<PodcastViewModel>
    {
        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.podcast, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource.GetText("ManageDownloads"));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_podcast_manage_downloads:
                    ViewModel.OptionCommand.Execute(ViewModel.Podcast);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override int FragmentId => Resource.Layout.fragment_tracklist;
    }
}