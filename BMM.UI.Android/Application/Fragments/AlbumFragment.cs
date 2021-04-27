using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.Content;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;
using Google.Android.Material.AppBar;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX;
using MvvmCross.Localization;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.AlbumFragment")]
    public class AlbumFragment : BaseFragment<AlbumViewModel>
    {
        MvxLanguageBinder DialogTextSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "UserDialogs");

        private string _toolbarTitle;

        public string ToolbarTitle
        {
            get=>  _toolbarTitle;

            set
            {
                _toolbarTitle = value;
                CollapsingToolbar.SetTitle(_toolbarTitle);
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            var set = this.CreateBindingSet<AlbumFragment, AlbumViewModel>();
            set.Bind().For(sa => sa.ToolbarTitle).To(vm => vm.Album.Title);
            set.Apply();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var swipeToRefresh = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);
            var appBar = view.FindViewById<AppBarLayout>(Resource.Id.appbar);

            if (appBar != null && swipeToRefresh != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;
            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.album, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource.GetText("AddAlbumToPlaylist"));
            menu.GetItem(1).SetTitle(ViewModel.TextSource.GetText("AddAlbumToMyTracks"));
            menu.GetItem(2).SetTitle(DialogTextSource.GetText("Album.Share"));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //ToDo: Use a more mvvm-like approach
                case Resource.Id.menu_AddAlbumToPlaylist:
                    ViewModel.AddToPlaylistCommand.Execute();
                    return true;
                case Resource.Id.menu_AddAlbumToMyTracks:
                    ViewModel.AddToMyTracksCommand.Execute();
                    return true;
                case Resource.Id.menu_ShareAlbum:
                    ViewModel.ShareCommand.Execute();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override int FragmentId => Resource.Layout.fragment_album;

        protected override Color ActionBarColor => new Color(ContextCompat.GetColor(this.Context, Android.Resource.Color.Transparent));
    }
}