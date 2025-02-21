using _Microsoft.Android.Resource.Designer;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Implementations.UI;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.CustomViews;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.Helpers.Gesture;
using BMM.UI.Droid.Application.Listeners;
using BMM.UI.Droid.Application.ViewHolders;
using JetBrains.Annotations;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Debug = System.Diagnostics.Debug;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.HvheDetailsFragment")]
    public class HvheDetailsFragment : BaseDialogFragment<HvheDetailsViewModel>
    {
        protected override int FragmentId => ResourceConstant.Layout.fragment_hvhe_details;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var recyclerView = view.FindViewById<MvxRecyclerView>(ResourceConstant.Id.HvheDetailsRecyclerView);
            var hvheChurchesSelectorView = view.FindViewById<HvheChurchesSelectorView>(ResourceConstant.Id.HvheChurchesSelectorView);
            
            recyclerView!.Adapter = new HvheDetailsRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
            recyclerView.AddOnScrollListener(new HvheDetailsScrollListener(
                isVisible =>
                {
                    hvheChurchesSelectorView!.Visibility = isVisible
                        ? ViewStates.Visible
                        : ViewStates.Gone;
                }));

            var set = this.CreateBindingSet<HvheDetailsFragment, HvheDetailsViewModel>();
            
            set.Apply();
            return view;
        }
    }
}