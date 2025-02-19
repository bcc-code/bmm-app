using _Microsoft.Android.Resource.Designer;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.Helpers.Gesture;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

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
            recyclerView!.Adapter = new HvheDetailsRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);

            var set = this.CreateBindingSet<HvheDetailsFragment, HvheDetailsViewModel>();
            
            set.Apply();
            return view;
        }
    }
}