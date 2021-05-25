using Android.Runtime;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.ContributorFragment")]
    public class ContributorFragment : BaseFragment<ContributorViewModel>
    {
        protected override MvxRecyclerAdapter CreateAdapter()
        {
            return new HeaderRecyclerAdapter((IMvxAndroidBindingContext)BindingContext);
        }

        protected override int FragmentId => Resource.Layout.fragment_tracklist;
    }
}