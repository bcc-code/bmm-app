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
        public override void OnStart()
        {
            base.OnStart();

            var set = this.CreateBindingSet<TrackCollectionFragment, TrackCollectionViewModel>();
            set.Bind(ParentActivity.SupportActionBar).For(sa => sa.Title).To(vm => vm.MyCollection.Name);
            set.Apply();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var swipeToRefresh = view.FindViewById<MvxSwipeRefreshLayout>(Resource.Id.refresher);
            var appBar = view.FindViewById<AppBarLayout>(Resource.Id.appbar);

            if (appBar != null && swipeToRefresh != null)
                appBar.OffsetChanged += (sender, args) => swipeToRefresh.Enabled = args.VerticalOffset == 0;

            InitRecyclerView(view);

            return view;
        }

        private void InitRecyclerView(View view)
        {
            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.trackcollection_recycler_view);
            if (recyclerView != null)
            {
                recyclerView.Adapter = new HeaderRecyclerAdapter((IMvxAndroidBindingContext) BindingContext);
                recyclerView.HasFixedSize = true;

                var layoutManager = new LinearLayoutManager(ParentActivity);
                recyclerView.SetLayoutManager(layoutManager);
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.trackcollection, menu);
            menu.GetItem(0).SetTitle(ViewModel.TextSource.GetText("MenuDelete"));
            menu.GetItem(1).SetTitle(ViewModel.TextSource.GetText("MenuEdit"));
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

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override int FragmentId => Resource.Layout.fragment_trackcollection;
    }
}