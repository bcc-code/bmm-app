using System;
using System.ComponentModel;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Content;
using BMM.Core.Constants;
using BMM.Core.Diagnostic.Interfaces;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Interactions;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Helpers;
using BMM.UI.Droid.Application.Helpers.BottomSheet;
using BMM.UI.Droid.Application.Helpers.Gesture;
using FFImageLoading;
using Google.Android.Material.BottomSheet;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxFragmentPresentation(typeof(MainActivityViewModel), Resource.Id.player_frame)]
    [Register("bmm.ui.droid.application.fragments.PlayerFragment")]
    public class PlayerFragment : BaseFragment<PlayerViewModel>, SeekBar.IOnSeekBarChangeListener
    {
        private const int TimeToCheckEmptyPlayerErrorInMillis = 5000;

        private BottomSheetManager _bottomSheetManager;
        private HorizontalSwipeDetector _swipeDetector;
        private ImageView _imageView;
        private SeekBar _seekBar;
        private IAnalytics _analytics;

        [Obsolete("We should use a more MVVM like approach and not handle with direct references like that")]
        private ConstraintLayout PlayerFragmentContainer { get; set; }

        public bool IsOpen => _bottomSheetManager.IsOpen;

        public bool IsVisible => PlayerFragmentContainer.Visibility == ViewStates.Visible;

        protected override int FragmentId => Resource.Layout.fragment_player;

        private IMvxInteraction<TogglePlayerInteraction> _interaction;
        private PreventBottomSheetChangesWhileSwipeHappens _preventBottomSheetChangesWhileSwipeHappens;

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

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.playback_history, menu);
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

            return view;
        }

        private bool CheckIfPlayerIsEmpty()
        {
            var titleTextView = View.FindViewById<TextView>(Resource.Id.title);
            return string.IsNullOrEmpty(titleTextView!.Text);
        }

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
            _seekBar = ParentActivity.FindViewById<SeekBar>(Resource.Id.player_seekbar);
            _seekBar.SetOnSeekBarChangeListener(this);
            _imageView = ParentActivity.FindViewById<ImageView>(Resource.Id.coverImagePlaceholder);

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

            if (allPropertiesHaveChanged || args.PropertyName == "CurrentTrack")
            {
                UpdateCover();
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
                SetLightNavigationBar();
        }

        private void UpdateCover()
        {
            Mvx.IoCProvider.Resolve<IExceptionHandler>()
                .FireAndForgetWithoutUserMessages(async () =>
                {
                    var coverImage = ViewModel.CurrentTrack?.ArtworkUri == null
                        ? await ImageService.Instance.LoadCompiledResource("placeholder_cover").AsBitmapDrawableAsync()
                        : await ImageService.Instance.LoadUrl(ViewModel.CurrentTrack?.ArtworkUri).AsBitmapDrawableAsync();

                    if (coverImage?.Bitmap == null)
                        return;

                    await Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>()
                        .ExecuteOnMainThreadAsync(() => { SetBlurredBackground(coverImage.Bitmap); });
                });
        }

        private void SetBlurredBackground(Bitmap cover)
        {
            var blurredCover = BitmapHelper.BlurImage(Context, cover);
            PlayerFragmentContainer.Background = new BitmapDrawable(blurredCover);

            FragmentBaseColor = BitmapHelper.GetColor(blurredCover);
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
            SetDarkNavigationBar();
        }

        public void HidePlayer()
        {
            PlayerFragmentContainer.Visibility = ViewStates.Invisible;
            UpdateStatusBarColor();
            SetLightNavigationBar();
        }

        public void OpenPlayer()
        {
            _bottomSheetManager.Open();

            UpdateCover();

            SetOnTouchListener();
            SetDarkNavigationBar();
        }

        public void ClosePlayer()
        {
            SetLightNavigationBar();
            _bottomSheetManager.Close();
        }

        private void UpdateStatusBarColor()
        {
            if (IsOpen && IsVisible)
            {
                SetStatusBarColor(FragmentBaseColor);
            }
            else
            {
                SetStatusBarColor(ColorOfUppermostFragment());
            }
        }

        private void SetLightNavigationBar()
        {
            if (Activity?.Window == null || Activity?.Resources == null || !SdkVersion.SupportsNavigationBarColors)
                return;
            Activity.Window.SetNavigationBarColor(new Color(ContextCompat.GetColor(Context, Resource.Color.white)));
            if (SdkVersion.SupportsNavigationBarDividerColor)
                Activity.Window.NavigationBarDividerColor = ContextCompat.GetColor(Context, Resource.Color.dark_gray);
        }

        private void SetDarkNavigationBar()
        {
            if (Activity?.Window == null || Activity?.Resources == null || !SdkVersion.SupportsNavigationBarColors)
                return;
            Activity.Window.SetNavigationBarColor(new Color(ContextCompat.GetColor(Context, Android.Resource.Color.Black)));
            if (SdkVersion.SupportsNavigationBarDividerColor)
                Activity.Window.NavigationBarDividerColor = ContextCompat.GetColor(Context, Android.Resource.Color.Transparent);
        }
    }
}