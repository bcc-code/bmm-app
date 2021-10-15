using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.ItemDecorators;
using FFImageLoading.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.CuratedPlaylistsFragment")]
    public class CuratedPlaylistsFragment : BaseCoversTileCollectionFragment<CuratedPlaylistsViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_browse_details_tiles;
    }
}