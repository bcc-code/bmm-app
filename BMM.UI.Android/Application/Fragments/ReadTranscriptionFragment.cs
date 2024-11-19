using _Microsoft.Android.Resource.Designer;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BMM.Core.Constants;
using BMM.Core.Models.Player.Lyrics;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Extensions;
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
        private LyricsLink _lyricsLink;
        private bool _isAutoTranscribed;
        private ImageView _aiImage;
        private TextView _headerLabel;

        protected override int FragmentId => Resource.Layout.fragment_read_transcriptions;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            _aiImage = view.FindViewById<ImageView>(ResourceConstant.Id.AIIcon);
            _headerLabel = view.FindViewById<TextView>(ResourceConstant.Id.HeaderLabel);
            
            return view;
        }

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
            
            set.Bind(this)
                .For(v => v.IsAutoTranscribed)
                .To(vm => vm.IsAutoTranscribed);
           
            set.Bind(this)
                .For(v => v.LyricsLink)
                .To(vm => vm.LyricsLink);

            set.Apply();
        }
        
        public LyricsLink LyricsLink
        {
            get => _lyricsLink;
            set
            {
                _lyricsLink = value;
                SetIconAndHeader();
            }
        }

        public bool IsAutoTranscribed
        {
            get => _isAutoTranscribed;
            set
            {
                _isAutoTranscribed = value;
                SetIconAndHeader();
            }
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

        private void SetIconAndHeader()
        {
            if (_isAutoTranscribed)
            {
                _aiImage.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.icon_ai));
            }
            else
            {
                int drawableId = LyricsLink?.LyricsLinkType == LyricsLinkType.SongTreasures
                    ? Resource.Drawable.image_song_treasures
                    : Resource.Drawable.icon_info;
                
                _aiImage.SetImageDrawable(Resources.GetDrawable(drawableId));
            }

            SetHeaderColor();
        }
        
        private void SetHeaderColor()
        {
            var colorId = _isAutoTranscribed
                ? ResourceConstant.Color.utility_auto_color
                : ResourceConstant.Color.label_one_color;

            _headerLabel.SetTextColor(Context.GetColorFromResource(colorId));
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