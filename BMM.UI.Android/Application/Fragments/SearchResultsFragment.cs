using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.Listeners;
using BMM.UI.Droid.Application.Listeners.Interfaces;
using FFImageLoading.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

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