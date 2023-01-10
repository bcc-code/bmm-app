using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.ViewPager.Widget;
using BMM.Core.Extensions;
using BMM.Core.Interactions.Base;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.CustomViews.TabLayout;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Helpers;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.SearchFragment")]
    public class SearchFragment : BaseFragment<SearchViewModel>, TextView.IOnEditorActionListener, View.IOnClickListener
    {
        private ViewPager _viewPager;
        private FlexibleWidthTabLayout _tabLayout;
        private TextView _searchTermEditText;
        private IBmmInteraction _removeFocusOnSearchInteraction;
        private ConstraintLayout _welcomeAndHistoryLayer;
        private MvxRecyclerView _recentSearchesRecyclerView;
        public BindableFragmentPagerAdapter<SearchResultsViewModel> ViewPagerAdapter { get; set; }
        
        protected override int FragmentId => Resource.Layout.fragment_search;
        
        public IBmmInteraction RemoveFocusOnSearchInteraction
        {
            get => _removeFocusOnSearchInteraction;
            set
            {
                if (_removeFocusOnSearchInteraction != null)
                    _removeFocusOnSearchInteraction.Requested -= OnRemoveFocusRequested;

                _removeFocusOnSearchInteraction = value;
                _removeFocusOnSearchInteraction.Requested += OnRemoveFocusRequested;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            _viewPager = view.FindViewById<ViewPager>(Resource.Id.ViewPager);
            _tabLayout = view.FindViewById<FlexibleWidthTabLayout>(Resource.Id.TabLayout);
            _welcomeAndHistoryLayer = view.FindViewById<ConstraintLayout>(Resource.Id.WelcomeAndHistoryLayer);
            _searchTermEditText = view.FindViewById<TextView>(Resource.Id.SearchTermEditText);
            _searchTermEditText.SetOnEditorActionListener(this);
            _welcomeAndHistoryLayer.SetOnClickListener(this);
            
            HasOptionsMenu = true;
            UpdateLayout();
            BindSearch();
            
            return view;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            _viewPager.PageSelected += ViewPagerOnPageSelected;
            DetachRemoveFocusInteraction();
            RemoveFocusOnSearchInteraction.Requested += OnRemoveFocusRequested;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            _viewPager.PageSelected -= ViewPagerOnPageSelected;
            DetachRemoveFocusInteraction();
        }

        private void DetachRemoveFocusInteraction()
        {
            if (RemoveFocusOnSearchInteraction == null)
                return;

            RemoveFocusOnSearchInteraction.Requested -= OnRemoveFocusRequested;
        }

        protected void BindSearch()
        {
            var set = this.CreateBindingSet<SearchFragment, SearchViewModel>();
            
            set.Bind(ViewPagerAdapter)
                .For(vp => vp.ItemsSource)
                .To(v => v.CollectionItems);

            set.Bind(_tabLayout)
                .For(v => v.ItemsSource)
                .To(vm => vm.CollectionItems);

            set.Bind(_tabLayout)
                .For(v => v.SelectedItem)
                .To(vm => vm.SelectedCollectionItem);

            set.Bind(this)
                .For(v => v.RemoveFocusOnSearchInteraction)
                .To(vm => vm.RemoveFocusOnSearchInteraction);
            
            set.Apply();
        }
        
        private void OnRemoveFocusRequested(object sender, EventArgs e)
        {
            ClearFocus();
        }

        private void ClearFocus()
        {
            _searchTermEditText.ClearFocus();
            var imm = (InputMethodManager)Activity.GetSystemService(Android.Content.Context.InputMethodService);
            imm.HideSoftInputFromWindow(_searchTermEditText.WindowToken, 0);
        }

        private void UpdateLayout()
        {
            ViewPagerAdapter = new BindableFragmentPagerAdapter<SearchResultsViewModel>(ChildFragmentManager, CreateFragmentForViewModel);
            _viewPager.Adapter = ViewPagerAdapter;
            _tabLayout.ViewPager = _viewPager;
        }

        private void ViewPagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            ViewModel.SelectedCollectionItem = ViewModel.CollectionItems[e.Position];
        }

        private BmmViewPagerFragmentInfo CreateFragmentForViewModel(SearchResultsViewModel viewModel)
        {
            var fragmentInfo = this.CreateFragmentForViewPager(
                viewModel.Title,
                viewModel.Title,
                typeof(SearchResultsFragment),
                viewModel);
            return fragmentInfo;
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            ClearFocus();
            
            if (actionId == ImeAction.Search) 
            {
                ViewModel.SearchCommand.ExecuteAsync().FireAndForget();
                return true;
            }

            return false;
        }

        public void OnClick(View v)
        {
            ClearFocus();
        }
    }
}