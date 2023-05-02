using Android.Runtime;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using Com.Airbnb.Lottie;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.HighlightedTextTrackFragment")]
    public class HighlightedTextTrackFragment : BaseDialogFragment<HighlightedTextTrackViewModel>
    {
        private bool _isCurrentlyPlaying;
        private LottieAnimationView _playAnimationView;
        protected override int FragmentId => Resource.Layout.fragment_highlighted_text_track;

        public bool IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set
            {
                _isCurrentlyPlaying = value;
                SetAnimationState();
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            _playAnimationView = view.FindViewById<LottieAnimationView>(Resource.Id.PlayAnimationView);
            var bottomContainer = view.FindViewById<ConstraintLayout>(Resource.Id.BottomContainer);
            bottomContainer!.TranslationZ = Resources.GetDimensionPixelSize(Resource.Dimension.margin_small);
            SetAnimationState();
            return view;
        }
        
        protected override void Bind()
        {
            var set = this.CreateBindingSet<HighlightedTextTrackFragment, HighlightedTextTrackViewModel>();
        
            set.Bind(this)
                .For(v => v.IsCurrentlyPlaying)
                .To(vm => vm.IsCurrentlyPlaying);
            
            set.Apply();
        }
        
        private void SetAnimationState()
        {
            _playAnimationView.Visibility = _isCurrentlyPlaying
                ? ViewStates.Visible
                : ViewStates.Invisible;
        }
    }
}