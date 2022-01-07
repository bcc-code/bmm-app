using System.ComponentModel;
using Android.Animation;
using Android.Graphics;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Graphics;
using BMM.Core.Constants;
using BMM.Core.Diagnostic.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Interactions;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.CustomViews;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Helpers;
using BMM.UI.Droid.Application.Helpers.BottomSheet;
using BMM.UI.Droid.Application.Helpers.Gesture;
using BMM.UI.Droid.Application.Listeners;
using FFImageLoading;
using Google.Android.Material.BottomSheet;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;
using ViewUtils = BMM.UI.Droid.Utils.ViewUtils;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.player_frame)]
    [Register("bmm.ui.droid.application.fragments.PlayerFragment")]
    public class PlayerFragment : BaseFragment<PlayerViewModel>, SeekBar.IOnSeekBarChangeListener
    {
        private const int TimeToCheckEmptyPlayerErrorInMillis = 2000;

        private BottomSheetManager _bottomSheetManager;
        private HorizontalSwipeDetector _swipeDetector;
        private ImageView _imageView;
        private SeekBar _seekBar;
        private IAnalytics _analytics;

        private ConstraintLayout PlayerFragmentContainer { get; set; }

        public bool IsOpen => _bottomSheetManager.IsOpen;

        public bool IsVisible => PlayerFragmentContainer.Visibility == ViewStates.Visible;

        protected override int FragmentId => Resource.Layout.fragment_player;

        private IMvxInteraction<TogglePlayerInteraction> _interaction;
        private PreventBottomSheetChangesWhileSwipeHappens _preventBottomSheetChangesWhileSwipeHappens;
        private Color _coverMainColor;
        private ShadowLayout _coverShadowLayout;
        private ImageView _coverImage;
        private Color _backgroundAccentColor;
        private TextView _titleLabel;
        private FrameLayout _coverContainer;
        private string _lastCoverUri;
        private float _coverTopPaddingMultiplier;
        private Bitmap _coverPlaceholder;

        protected override bool ShouldClearMenuItemsAtStart => false;

        public IMvxInteraction<TogglePlayerInteraction> Interaction
        {
            get => _interaction;
            set
            {
                if (_interaction != null)
                    _interaction.Requested -= OnTogglePlayerInteraction;

                _interaction = value;
                _interaction.Requested += OnTogglePlayerInteraction;
            }
        }

        private void OnTogglePlayerInteraction(object sender, MvxValueEventArgs<TogglePlayerInteraction> args)
        {
            if (args.Value.Open)
                OpenPlayer();
            else
                ClosePlayer();
        }

        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            if (!fromUser)
                return;

            ViewModel.SliderPosition = progress;
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {
            ViewModel.IsSeeking = true;
        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            ViewModel.SliderPosition = seekBar.Progress;
            ViewModel.IsSeeking = false;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _swipeDetector = new HorizontalSwipeDetector();
            _analytics = Mvx.IoCProvider.Resolve<IAnalytics>();

            var view = base.OnCreateView(inflater, container, savedInstanceState);

            PlayerFragmentContainer = view.FindViewById<ConstraintLayout>(Resource.Id.playerFragment_container);

            var bottomSheet = BottomSheetBehavior.From(PlayerFragmentContainer);
            _bottomSheetManager = new BottomSheetManager(bottomSheet);
            bottomSheet.SetBottomSheetCallback(_bottomSheetManager);

            _preventBottomSheetChangesWhileSwipeHappens =
                new PreventBottomSheetChangesWhileSwipeHappens(_bottomSheetManager, _swipeDetector);

            UpdateStatusBarColor();
            Title = StringConstants.Space;

            var timeDiagnosticTool = Mvx.IoCProvider.Resolve<ITimeDiagnosticTool>();

            timeDiagnosticTool
                .LogIfConditionIsTrueAfterSpecifiedTime(
                    CheckIfPlayerIsEmpty,
                    TimeToCheckEmptyPlayerErrorInMillis,
                    Event.EmptyPlayer);

            _coverContainer = view.FindViewById<FrameLayout>(Resource.Id.CoverContainer);
            _coverShadowLayout = view.FindViewById<ShadowLayout>(Resource.Id.CoverShadowLayout);
            _coverImage = view.FindViewById<ImageView>(Resource.Id.CoverImagePlaceholder);
            _backgroundAccentColor = Context.GetColorFromResource(Resource.Color.background_secondary_color);
            _titleLabel = view.FindViewById<TextView>(Resource.Id.TitleLabel);
            _coverContainer!.ClipToOutline = true;
            
            var subtitleLabel = view.FindViewById<TextView>(Resource.Id.SubtitleLabel);
            subtitleLabel!.Selected = true;
            view.Post(SetSizes);

            return view;
        }

        private bool CheckIfPlayerIsEmpty() => string.IsNullOrEmpty(_titleLabel?.Text);

        public override void OnStart()
        {
            base.OnStart();
            SetOnTouchListener();

            var set = this.CreateBindingSet<PlayerFragment, PlayerViewModel>();
            set.Bind(this).For(view => view.Interaction).To(vm => vm.ClosePlayerInteraction).OneWay();
            set.Apply();
            _analytics.LogEvent("PlayerFragment - OnStart()");
        }

        public override void OnStop()
        {
            base.OnStop();

            Interaction.Requested -= OnTogglePlayerInteraction;
            _analytics.LogEvent("PlayerFragment - OnStop()");
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            _seekBar = ParentActivity.FindViewById<SeekBar>(Resource.Id.PlayerSeekbar);
            _seekBar.SetOnSeekBarChangeListener(this);
            _imageView = ParentActivity.FindViewById<ImageView>(Resource.Id.CoverImagePlaceholder);

            UpdateStatusBarColor();

            base.OnViewCreated(view, savedInstanceState);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _analytics.LogEvent("PlayerFragment - OnDestroy()");
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            _bottomSheetManager.OnBottomSheetStateChanged += HandleBottomSheetStateChanged;
            _swipeDetector.OnSwipeDetected += HandleSwipe;
            _preventBottomSheetChangesWhileSwipeHappens.Register();
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _bottomSheetManager.OnBottomSheetStateChanged -= HandleBottomSheetStateChanged;
            _swipeDetector.OnSwipeDetected -= HandleSwipe;
            _preventBottomSheetChangesWhileSwipeHappens.Unregister();
        }

        public void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var propertyName = args.PropertyName;
            var allPropertiesHaveChanged = string.IsNullOrEmpty(propertyName);

            if (allPropertiesHaveChanged || args.PropertyName == nameof(ViewModel.CurrentTrack))
            {
                UpdateCover();
                UpdateTitleSize();
            }
        }

        private void HandleSwipe(object sender, SwipeEvent swipeEvent)
        {
            if (swipeEvent.EventType == SwipeEventType.End)
            {
                IMvxCommand command = null;

                if (swipeEvent.Direction == SwipeDirection.Right)
                {
                    command = ViewModel.NextCommand;
                }
                else if (swipeEvent.Direction == SwipeDirection.Left)
                {
                    command = ViewModel.PreviousCommand;
                }

                if (command != null && command.CanExecute())
                {
                    command.Execute();
                }
            }
        }

        private void HandleBottomSheetStateChanged(object sender, int state)
        {
            UpdateStatusBarColor();
            if (state == BottomSheetBehavior.StateHidden)
                ViewUtils.SetDefaultNavigationBarColor(Activity);
        }

        private void UpdateCover()
        {
            Mvx.IoCProvider.Resolve<IExceptionHandler>()
                .FireAndForgetWithoutUserMessages(async () =>
                {
                    Bitmap coverBitmap = null;
                    
                    if (_lastCoverUri == ViewModel.CurrentTrack?.ArtworkUri)
                        return;
                    
                    _lastCoverUri = ViewModel.CurrentTrack?.ArtworkUri;

                    if (!string.IsNullOrEmpty(_lastCoverUri))
                    {
                        var coverImage = await ImageService.Instance.LoadUrl(ViewModel.CurrentTrack?.ArtworkUri).AsBitmapDrawableAsync();
                        coverBitmap = coverImage.Bitmap;
                    }

                    _coverPlaceholder ??= GetBitmapFromVectorDrawable(Resource.Drawable.new_placeholder_cover);
                    
                    bool shouldTintBackground = coverBitmap != null;
                    coverBitmap ??= _coverPlaceholder;

                    await Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>()
                        .ExecuteOnMainThreadAsync(() => { SetCover(coverBitmap, shouldTintBackground); });
                });
        }

        public Bitmap GetBitmapFromVectorDrawable(int drawableId)
        {
            var drawable = Resources.GetDrawable(drawableId, Activity!.Application!.Theme);
            
            var bitmap = Bitmap.CreateBitmap(drawable!.IntrinsicWidth,
                drawable.IntrinsicHeight,
                Bitmap.Config.Argb8888);

            var canvas = new Canvas(bitmap);
            drawable.SetBounds(0,
                0,
                canvas.Width,
                canvas.Height);
            drawable.Draw(canvas);
            
            return bitmap;
        }

        private void SetCover(Bitmap cover, bool shouldTintBackground)
        {
            _imageView.SetImageBitmap(cover);

            _coverMainColor = BitmapHelper.GetMutedColor(cover);
            cover.Dispose();
            
            var coverMainColorWithAlpha = new Color(_coverMainColor)
            {
                A = 170
            };
            
            if (shouldTintBackground)
                _coverShadowLayout.SetShadowColor(coverMainColorWithAlpha);
            else
                _coverShadowLayout.SetShadowColor(Color.Transparent);
            
            int accentColor = shouldTintBackground ?
                ColorUtils.BlendARGB(
                Context.GetColorFromResource(Resource.Color.background_secondary_color).ToArgb(),
                _coverMainColor.ToArgb(),
                0.1f)
                : Context.GetColorFromResource(Resource.Color.background_secondary_color);

            var newBackgroundAccentColor = new Color(accentColor);
            var animation = ValueAnimator.OfArgb(_backgroundAccentColor, newBackgroundAccentColor);

            animation!.AddUpdateListener(new AnimationUpdateListener(valueAnimator =>
            {
                var newColor = new Color((int)valueAnimator.AnimatedValue);
                PlayerFragmentContainer.SetBackgroundColor(newColor);
                
                if (IsOpen && IsVisible)
                    ViewUtils.SetSpecifiedNavigationBarColor(Activity, newColor);
            }));

            animation.SetDuration(ViewConstants.LongAnimationDurationInMilliseconds);
            animation.Start();

            _backgroundAccentColor = newBackgroundAccentColor;
            UpdateStatusBarColor();
        }

        private void SetOnTouchListener()
        {
            _imageView.SetOnTouchListener(_swipeDetector);
        }

        public void ShowPlayer()
        {
            PlayerFragmentContainer.Visibility = ViewStates.Visible;
            UpdateStatusBarColor();
            ViewUtils.SetSpecifiedNavigationBarColor(Activity, _backgroundAccentColor);
        }

        public void HidePlayer()
        {
            PlayerFragmentContainer.Visibility = ViewStates.Invisible;
            UpdateStatusBarColor();
            ViewUtils.SetDefaultNavigationBarColor(Activity);
        }

        public void OpenPlayer()
        {
            _bottomSheetManager.Open();
            UpdateCover();
            SetOnTouchListener();
            ViewUtils.SetSpecifiedNavigationBarColor(Activity, _backgroundAccentColor);
        }

        public void ClosePlayer()
        {
            ViewUtils.SetDefaultNavigationBarColor(Activity);
            _bottomSheetManager.Close();
        }

        private void UpdateStatusBarColor()
        {
            if (IsOpen && IsVisible)
                SetStatusBarColor(_backgroundAccentColor);
            else
                SetStatusBarColor(ColorOfUppermostFragment());
        }

        private void UpdateTitleSize()
        {
            View.Post(() =>
            {
                int coverBottom = _coverContainer!.Bottom;
                int titleLabelBottom = _titleLabel.Bottom;
                int margin = Resources.GetDimensionPixelSize(Resource.Dimension.margin_xmedium);

                int desiredTitleLabelHeight = titleLabelBottom - coverBottom - margin - _coverShadowLayout.PaddingTop;
                _titleLabel.UpdateHeight(desiredTitleLabelHeight);
            });
        }

        private void SetSizes()
        {
            float coverSizeMultiplier = 0.4f;

            float widthToHeightRatio = View.Width / (float)View.Height;
            
            if (View.Height <= 1280)
                _coverTopPaddingMultiplier = 0.5f;
            else if (View.Height <= 1920)
                _coverTopPaddingMultiplier = 0.75f;
            else
            {
                if (widthToHeightRatio > 0.8f)
                {
                    _coverTopPaddingMultiplier = 0.5f;
                    coverSizeMultiplier = 0.3f;
                }
                else
                {
                    _coverTopPaddingMultiplier = 1f;
                    coverSizeMultiplier = 0.45f;
                }
            }

            _coverShadowLayout.TopPaddingMultiplier = _coverTopPaddingMultiplier;
            _coverShadowLayout!.SetShadowRadius(View.Width * 0.25f);
            _coverImage.UpdateSize((int)(View.Width * coverSizeMultiplier));
            UpdateTitleSize();
        }
    }
}