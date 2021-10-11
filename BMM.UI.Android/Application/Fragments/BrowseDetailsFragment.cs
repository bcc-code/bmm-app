using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.BrowseDetailsFragment")]
    public class BrowseDetailsFragment : BaseFragment<BrowseDetailsViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_browse_details;

        protected override bool HasCustomTitle => true;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            Title = ViewModel.Title;
            return view;
        }

        protected override void Bind()
        {
            base.Bind();

            var set = this.CreateBindingSet<BrowseDetailsFragment, BrowseDetailsViewModel>();

            set.Bind(this)
                .For(v => v.Title)
                .To(vm => vm.Title);

            set.Apply();
        }
    }
}