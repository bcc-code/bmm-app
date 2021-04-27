using Android.Graphics;
using Android.Runtime;
using AndroidX.Core.Content;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.content_frame, true)]
    [Register("bmm.ui.droid.application.fragments.ContributorFragment")]
    public class ContributorFragment : BaseFragment<ContributorViewModel>
    {
        private string _toolbarTitle;

        public string ToolbarTitle
        {
            get
            {
                return _toolbarTitle;
            }
            set
            {
                _toolbarTitle = value;
                CollapsingToolbar.SetTitle(_toolbarTitle);
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            var set = this.CreateBindingSet<ContributorFragment, ContributorViewModel>();
            set.Bind().For(sa => sa.ToolbarTitle).To(vm => vm.Contributor.Name);
            set.Apply();
        }

        protected override Color ActionBarColor => new Color(ContextCompat.GetColor(this.Context, Android.Resource.Color.Transparent));

        protected override int FragmentId => Resource.Layout.fragment_contributor;
    }
}