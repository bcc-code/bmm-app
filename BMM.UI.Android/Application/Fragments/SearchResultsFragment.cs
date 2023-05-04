using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.Fragments
{
    [Register("bmm.ui.droid.application.fragments.SearchResultsFragment")]
    public class SearchResultsFragment : BaseFragment<SearchResultsViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_search_results;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            return view;
        }

        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new SearchResultsRecyclerAdapter((IMvxAndroidBindingContext) BindingContext);
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            RecyclerView.ScrollChange += ClearSearchFocus;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            RecyclerView.ScrollChange += ClearSearchFocus;
        }
        
        private void ClearSearchFocus(object sender, View.ScrollChangeEventArgs e)
        {
            ViewModel.ClearFocusAction?.Invoke();
        }
    }
}