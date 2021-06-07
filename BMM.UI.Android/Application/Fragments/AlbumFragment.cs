using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using Google.Android.Material.AppBar;
using MvvmCross.DroidX;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Localization;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.AlbumFragment")]
    public class AlbumFragment : BaseFragment<AlbumViewModel>
    {
        MvxLanguageBinder DialogTextSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "UserDialogs");

        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.album, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource.GetText("AddAlbumToPlaylist"));
            menu.GetItem(1).SetTitle(DialogTextSource.GetText("Album.Share"));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //ToDo: Use a more mvvm-like approach
                case Resource.Id.menu_AddAlbumToPlaylist:
                    ViewModel.AddToPlaylistCommand.Execute();
                    return true;
                case Resource.Id.menu_ShareAlbum:
                    ViewModel.ShareCommand.Execute();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override int FragmentId => Resource.Layout.fragment_tracklist;
    }
}