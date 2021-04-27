using Android.OS;
using Android.Runtime;
using Android.Views;
using BMM.Core.ViewModels;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.miniplayer_frame)]
    [Register("bmm.ui.droid.application.fragments.MiniPlayerFragment")]
    public class MiniPlayerFragment : MvxFragment<MiniPlayerViewModel>
	{
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return this.BindingInflate(Resource.Layout.fragment_miniplayer, null);
        }
	}
}