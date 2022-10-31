using System;
using System.ComponentModel;
using Android.Animation;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Graphics;
using BMM.Core.Constants;
using BMM.Core.Diagnostic.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Interactions;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Constants;
using BMM.UI.Droid.Application.Constants.Player;
using BMM.UI.Droid.Application.CustomViews;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Helpers;
using BMM.UI.Droid.Application.Helpers.BottomSheet;
using BMM.UI.Droid.Application.Helpers.Gesture;
using BMM.UI.Droid.Application.Listeners;
using FFImageLoading.Drawables;
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
        private const int BackgroundAccentAlpha = 170;
        private const float BackgroundAccentColorPercentageVolume = 0.1f;
        private const string NotNightModePlaceholderCover = "res:new_placeholder_cover";
        private const string NightModePlaceholderCover = "res:new_placeholder_cover_night";

        private BottomSheetManager _bottomSheetManager;
        private HorizontalSwipeDetector _swipeDetector;
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
        private BmmCachedImageView _coverImage;
        private Color _backgroundAccentColor;
        private TextView _titleLabel;
        private FrameLayout _coverContainer;
        private float _coverTopPaddingMultiplier;
        private string _coverImagePath;

        protected override bool ShouldClearMenuItemsAtStart => false;
        
        private float DefaultCoverShadowRadiusSize => View.Width * 0.25f;
        
        private string CoverPlaceholderPath { get; set; }

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
            _bottomSheetManager.Close();

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
            _coverImage = view.FindViewById<BmmCachedImageView>(Resource.Id.CoverImageView);
            _backgroundAccentColor = Context.GetColorFromResource(Resource.Color.background_secondary_color);
            _titleLabel = view.FindViewById<TextView>(Resource.Id.TitleLabel);
            _coverContainer!.ClipToOutline = true;
            
            var subtitleLabel = view.FindViewById<TextView>(Resource.Id.SubtitleLabel);
            subtitleLabel!.Selected = true;
            view.Post(SetSizes);
            SetupCoverImage();
            BindCover();
                
            return view;
        }
        
        private void SetupCoverImage()
        {
            CoverPlaceholderPath = Context.IsNightMode()
                ? NightModePlaceholderCover
                : NotNightModePlaceholderCover;

            _coverImage.LoadingPlaceholderImagePath = CoverPlaceholderPath;
            _coverImage.ErrorPlaceholderImagePath = CoverPlaceholderPath;
        }

        private void BindCover()
        {
            var set = this.CreateBindingSet<PlayerFragment, PlayerViewModel>();

            set.Bind(this)
                .For(v => v.CoverImagePath)
                .To(vm => vm.CurrentTrack.ArtworkUri);
            
            set.Apply();
        }

        public string CoverImagePath
        {
            get => _coverImagePath;
            set
            {
                _coverImagePath = value;
                _coverImage.ImagePath = string.IsNullOrEmpty(_coverImagePath)
                    ? CoverPlaceholderPath
                    : _coverImagePath;
            }
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
            _coverImage.ImageChanged += CoverImageOnImageChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _bottomSheetManager.OnBottomSheetStateChanged -= HandleBottomSheetStateChanged;
            _swipeDetector.OnSwipeDetected -= HandleSwipe;
            _preventBottomSheetChangesWhileSwipeHappens.Unregister();
            _coverImage.ImageChanged -= CoverImageOnImageChanged;
        }

        public void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            string propertyName = args.PropertyName;
            bool allPropertiesHaveChanged = string.IsNullOrEmpty(propertyName);

            if (allPropertiesHaveChanged || args.PropertyName == nameof(ViewModel.CurrentTrack))
                UpdateTitleSize();
        }

        private void CoverImageOnImageChanged(object sender, EventArgs e)
        {
            if (!(_coverImage.Drawable is SelfDisposingBitmapDrawable selfDisposingBitmapDrawable)
                || string.IsNullOrEmpty(selfDisposingBitmapDrawable.InCacheKey))
            {
                SetCoverShadow(true);
                return;
            }

            SetCoverShadow(!CoverPlaceholderPath.Contains(selfDisposingBitmapDrawable.InCacheKey));
        }

        private void HandleSwipe(object sender, SwipeEvent swipeEvent)
        {
            if (swipeEvent.EventType == SwipeEventType.End)
            {
                IMvxCommand command = null;

                if (swipeEvent.Direction == SwipeDirection.Right)
                    command = ViewModel.NextCommand;
                else if (swipeEvent.Direction == SwipeDirection.Left)
                    command = ViewModel.PreviousCommand;

                if (command != null && command.CanExecute())
                    command.Execute();
            }
        }

        private void HandleBottomSheetStateChanged(object sender, int state)
        {
            UpdateStatusBarColor();
            if (state == BottomSheetBehavior.StateHidden)
                ViewUtils.SetDefaultNavigationBarColor(Activity);
            
            if (state == BottomSheetBehavior.StateExpanded)
                View?.Post(SetSizes);
        }

        private void SetCoverShadow(bool shouldTintBackground)
        {
            if (!(_coverImage.Drawable is BitmapDrawable bitmapDrawable))
                return;

            _coverMainColor = BitmapHelper.GetDominantColor(bitmapDrawable.Bitmap);
            
            var coverMainColorWithAlpha = new Color(_coverMainColor)
            {
                A = BackgroundAccentAlpha
            };
            
            if (shouldTintBackground)
                _coverShadowLayout.SetShadowColor(coverMainColorWithAlpha);
            else
                _coverShadowLayout.SetShadowColor(Color.Transparent);
            
            int accentColor = shouldTintBackground ?
                ColorUtils.BlendARGB(
                Context.GetColorFromResource(Resource.Color.background_secondary_color).ToArgb(),
                _coverMainColor.ToArgb(),
                BackgroundAccentColorPercentageVolume)
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

        private void SetOnTouchListener() => _coverImage.SetOnTouchListener(_swipeDetector);

        private void OpenPlayer()
        {
            _bottomSheetManager.Open();
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
            View?.Post(() =>
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
            if (View == null)
                return;
            
            float coverSizeMultiplier = CoverConstants.CoverSizeMultiplierConstants.Medium;

            if (View.Height <= AndroidScreenSizesHeight.HD)
                _coverTopPaddingMultiplier = CoverConstants.CoverTopPaddingMultiplierConstants.Small;
            else if (View.Height <= AndroidScreenSizesHeight.FullHD)
                _coverTopPaddingMultiplier = CoverConstants.CoverTopPaddingMultiplierConstants.Medium;
            else
            {
                if (View.IsPortrait())
                {
                    _coverTopPaddingMultiplier = CoverConstants.CoverTopPaddingMultiplierConstants.Big;
                    coverSizeMultiplier = CoverConstants.CoverSizeMultiplierConstants.Big;
                }
                else
                {
                    _coverTopPaddingMultiplier = CoverConstants.CoverTopPaddingMultiplierConstants.Small;
                    coverSizeMultiplier = CoverConstants.CoverSizeMultiplierConstants.Small;
                }
            }

            _coverShadowLayout.TopPaddingMultiplier = _coverTopPaddingMultiplier;
            _coverShadowLayout!.SetShadowRadius(DefaultCoverShadowRadiusSize);
            _coverImage.UpdateSize((int)(View.Width * coverSizeMultiplier));
            
            UpdateTitleSize();
        }
    }
}