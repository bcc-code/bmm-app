using _Microsoft.Android.Resource.Designer;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Fragments.Base;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = true, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.QueueFragment")]
    public class QueueFragment : BaseDialogFragment<QueueViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_queue;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var recyclerView = view.FindViewById<MvxRecyclerView>(ResourceConstant.Id.QueueRecyclerView);
            recyclerView.Adapter = new QueueRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
            
            return view;
        }
    }
}