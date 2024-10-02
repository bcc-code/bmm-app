using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.TrackCollectionFragment")]
    public class TrackCollectionFragment : BaseFragment<TrackCollectionViewModel>
    {
        private bool _canEdit;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view =  base.OnCreateView(inflater, container, savedInstanceState);

            var set = this.CreateBindingSet<TrackCollectionFragment, TrackCollectionViewModel>();

            set
                .Bind(this)
                .For(v => v.CanEdit)
                .To(vm => vm.CanEdit);

            set.Apply();
            return view;
        }

        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                _canEdit = value;
                Activity.InvalidateOptionsMenu();
            }
        }

        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);

            if (CanEdit)
                CreateMenuIfPrivate(menu, inflater);
            else
                CreateMenuIfShared(menu, inflater);
        }

        private void CreateMenuIfPrivate(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.trackcollection, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource[Translations.UserDialogs_AddAllToPlaylist]);
            menu.GetItem(1).SetTitle(ViewModel.TextSource[Translations.TrackCollectionViewModel_SharePlaylist]);
            menu.GetItem(2).SetTitle(ViewModel.TextSource[Translations.TrackCollectionViewModel_DeletePlaylist]);
            menu.GetItem(3).SetTitle(ViewModel.TextSource[Translations.TrackCollectionViewModel_EditPlaylist]);
        }

        private void CreateMenuIfShared(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.trackcollection_shared, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource[Translations.UserDialogs_AddAllToPlaylist]);
            menu.GetItem(1).SetTitle(ViewModel.TextSource[Translations.TrackCollectionViewModel_RemovePlaylist]);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_delete:
                    ViewModel.DeleteCommand.Execute();
                    return true;

                case Resource.Id.menu_edit:
                    ViewModel.EditCommand.Execute();
                    return true;

                case Resource.Id.menu_share:
                    ViewModel.ShareCommand.Execute();
                    return true;

                case Resource.Id.menu_remove:
                    ViewModel.RemoveCommand.Execute();
                    return true;
                
                case Resource.Id.menu_add_to_trackcollection:
                    ViewModel.AddToTrackCollectionCommand.Execute();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override int FragmentId => Resource.Layout.fragment_tracklist;
    }
}