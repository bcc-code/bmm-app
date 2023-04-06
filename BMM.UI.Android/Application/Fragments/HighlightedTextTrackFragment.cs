using Android.Animation;
using Android.Runtime;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using FFImageLoading.Extensions;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.HighlightedTextTrackFragment")]
    public class HighlightedTextTrackFragment : BaseDialogFragment<HighlightedTextTrackViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_highlighted_text_track;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            var bottomContainer = view.FindViewById<ConstraintLayout>(Resource.Id.BottomContainer);
            bottomContainer!.TranslationZ = 8.DpToPixels();
            return view;
        }
    }
}