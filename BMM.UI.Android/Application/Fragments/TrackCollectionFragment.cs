using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using Google.Android.Material.AppBar;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.TrackCollectionFragment")]
    public class TrackCollectionFragment : BaseFragment<TrackCollectionViewModel>
    {
        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.trackcollection, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource.GetText("MenuShare"));
            menu.GetItem(1).SetTitle(ViewModel.TextSource.GetText("MenuDelete"));
            menu.GetItem(2).SetTitle(ViewModel.TextSource.GetText("MenuEdit"));
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

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override int FragmentId => Resource.Layout.fragment_tracklist;
    }
}