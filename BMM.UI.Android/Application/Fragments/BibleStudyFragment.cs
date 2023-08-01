using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.Fragments.Base;
using Com.Github.Jinatonic.Confetti;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.BibleStudyFragment")]
    public class BibleStudyFragment : BaseDialogFragment<BibleStudyViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_bible_study;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.BibleStudyRecyclerView);
            recyclerView!.Adapter = new BibleStudyRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);

            return view;
        }
    }
}