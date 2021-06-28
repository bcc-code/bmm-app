using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Activity;
using BMM.Core.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.ShareTrackCollectionFragment")]
    public class ShareTrackCollectionFragment : BaseFragment<ShareTrackCollectionViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_share_trackcollection;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            //Activity?.OnBackPressedDispatcher.AddCallback(this, new EditTrackCollectionOnBackPressedCallback(ViewModel));
            ParentActivity?.SupportActionBar?.SetHomeAsUpIndicator(Resource.Drawable.icon_close_static);

            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.share_trackcollection, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource.GetText("Done"));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_done:
                    ViewModel.CloseCommand.Execute();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private class EditTrackCollectionOnBackPressedCallback : OnBackPressedCallback
        {
            private readonly EditTrackCollectionViewModel _viewModel;

            public EditTrackCollectionOnBackPressedCallback(EditTrackCollectionViewModel viewModel) : base(true)
            {
                _viewModel = viewModel;
            }

            public override void HandleOnBackPressed()
            {
                _viewModel.DiscardAndCloseCommand.Execute();
            }
        }
    }
}