using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.Core.Content.Resources;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using SearchView = AndroidX.AppCompat.Widget.SearchView;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.SearchFragment")]
    public class SearchFragment : BaseFragment<SearchViewModel>
    {
        private SearchView _searchView;
        private MvxRecyclerView _recyclerView;

        protected override int FragmentId => Resource.Layout.fragment_search;

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.search, menu);

            var searchMenu = menu.FindItem(Resource.Id.action_search);

            Unsubscribe();

            _searchView = searchMenu.ActionView as SearchView;
            _searchView.QueryHint = ViewModel.TextSource[Translations.SearchViewModel_SearchHint];

            //Workaround for tablets to show full screen
            _searchView.MaxWidth = 100000;

            var searchText = _searchView.FindViewById<EditText>(Resource.Id.search_src_text);

            var set = this.CreateBindingSet<SearchFragment, SearchViewModel>();
            set.Bind(searchText).For(sa => sa.Text).To(vm => vm.SearchTerm);
            set.Apply();

            searchText!.SetTextColor(Context.GetColorFromResource(Resource.Color.label_primary_color));
            searchText!.SetHintTextColor(Context.GetColorFromResource(Resource.Color.label_secondary_color));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                searchText!.SetTextCursorDrawable(Resource.Drawable.edit_text_cursor);

            _searchView.OnActionViewExpanded();
            _searchView.ClearFocus();
            Subscribe();
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            Subscribe();
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_searchView == null)
                return;

            _searchView.QueryTextSubmit -= SearchViewOnQueryTextSubmit;
            _searchView.QueryTextSubmit += SearchViewOnQueryTextSubmit;
        }

        private void Unsubscribe()
        {
            if (_searchView == null)
                return;

            _searchView.QueryTextSubmit -= SearchViewOnQueryTextSubmit;
        }

        private void SearchViewOnQueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            ViewModel.SearchCommand.Execute();
            _searchView.ClearFocus();
            e.Handled = true;
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {
            base.OnPrepareOptionsMenu(menu);

            if (ViewModel.SearchTerm == string.Empty)
                ViewModel.SearchTerm = SearchViewModel.SearchedString;

            if (!string.IsNullOrEmpty(ViewModel.SearchTerm))
                _searchView.ClearFocus();
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ViewModel.OnFocusLoose = () => _searchView?.ClearFocus();

            // currently not possible to bind scrolling inside of the axml
            // see supported bindings: https://www.mvvmcross.com/documentation/platform/android/android-recyclerview
            _recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.my_recycler_view);
        }

        public override void OnStart()
        {
            base.OnStart();
            _recyclerView.ScrollChange += ClearSearchFocus;
        }

        public override void OnPause()
        {
            base.OnPause();
            _recyclerView.ScrollChange -= ClearSearchFocus;
        }

        private void ClearSearchFocus(object sender, View.ScrollChangeEventArgs e)
        {
            _searchView?.ClearFocus();
        }
    }
}