using Android.Runtime;
using AndroidX.RecyclerView.Widget;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Fragments.Base;
using BMM.UI.Droid.Application.ItemDecorators;
using BMM.UI.Droid.Application.LayoutManagers;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.ReadTranscriptionFragment")]
    public class ReadTranscriptionFragment : BaseDialogFragment<ReadTranscriptionViewModel>
    {
        private IMvxInteraction<int> _adjustScrollPositionInteraction;
        private MvxRecyclerView _recyclerView;
        private LinearLayoutManager _centerLayoutManager;
        private int _lastPosition;
        private bool _initialized;
        protected override int FragmentId => Resource.Layout.fragment_read_transcriptions;

        protected override void Bind()
        {
            base.Bind();

            var set = this.CreateBindingSet<ReadTranscriptionFragment, ReadTranscriptionViewModel>();
            
            _recyclerView = FragmentView!.FindViewById<MvxRecyclerView>(Resource.Id.ReadTranscriptionsRecyclerView);
            _centerLayoutManager = new CenterLayoutManager(_recyclerView!.Context, LinearLayoutManager.Vertical, false);
            _recyclerView.SetLayoutManager(_centerLayoutManager);
            _recyclerView.AddItemDecoration(new BottomOffsetDecoration(Resources.GetDimensionPixelSize(Resource.Dimension.read_transcription_bottom_offset)));

            set.Bind(_recyclerView)
                .For(v => v.ItemsSource)
                .To(vm => vm.Transcriptions);
            
            set.Bind(this)
                .For(f => f.AdjustScrollPositionInteraction)
                .To(vm => vm.AdjustScrollPositionInteraction)
                .OneWay();

            set.Apply();
        }

        public IMvxInteraction<int> AdjustScrollPositionInteraction
        {
            get => _adjustScrollPositionInteraction;
            set
            {
                if (_adjustScrollPositionInteraction != null)
                    _adjustScrollPositionInteraction.Requested -= OnAdjustScrollPositionInteractionRequested;

                _adjustScrollPositionInteraction = value;
                _adjustScrollPositionInteraction.Requested += OnAdjustScrollPositionInteractionRequested;
            }
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            if (_adjustScrollPositionInteraction != null)
            {
                _adjustScrollPositionInteraction.Requested -= OnAdjustScrollPositionInteractionRequested;
                _adjustScrollPositionInteraction.Requested += OnAdjustScrollPositionInteractionRequested;
            }
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();

            if (_adjustScrollPositionInteraction != null)
                _adjustScrollPositionInteraction.Requested -= OnAdjustScrollPositionInteractionRequested;
        }

        private void OnAdjustScrollPositionInteractionRequested(object sender, MvxValueEventArgs<int> e)
        {
            if (_lastPosition == e.Value)
                return;
            
            _lastPosition = e.Value;

            if (!_initialized)
            {
                FragmentView.Post(() => _centerLayoutManager.ScrollToPositionWithOffset(e.Value, _recyclerView.MeasuredHeight / 2));
                _initialized = true;
            }
            else
            {
                FragmentView.Post(() => _recyclerView.SmoothScrollToPosition(e.Value));
            }
        }
    }
}