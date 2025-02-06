using Android.Animation;
using Android.Content;
using Android.Runtime;
using Android.Views;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.Models.Enums;
using BMM.UI.Droid.Application.Adapters.Swipes;
using BMM.UI.Droid.Application.CustomViews.Swipes;
using MvvmCross.Commands;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace BMM.UI.Droid.Application.ViewHolders.Base
{
    public abstract class SwipeMenuViewHolder : MvxRecyclerViewHolder, View.IOnTouchListener
    {
        private const int ItemWidth = 80;

        protected readonly ISwipeMenuAdapter SwipeMenuAdapter;
        public const int MinSwipeThreshold = 10;
        public const float MinStuckThreshold = 0.9f;
        public const int AnimationDuration = 300;

        private View _mainItemView;
        private ValueAnimator _animator;
        private float _touchStartingPointX;
        private float _touchInitialOffsetX;
        private bool _menuInitialized;
        private bool _touchMoved;
        private bool _touchStarted;

        public SwipeMenuControl LeftMenu { get; private set; }
        public SwipeMenuControl RightMenu { get; private set; }
        public bool IsSwipingEnabled { get; set; } = true;

        protected Context Context { get; }

        public SwipeMenuViewHolder(
            View itemView,
            IMvxAndroidBindingContext context,
            ISwipeMenuAdapter swipeMenuAdapter)
            : base(itemView, context)
        {
            SwipeMenuAdapter = swipeMenuAdapter;
            Context = itemView.Context;
            WrapView();
        }

        private void WrapView()
        {
            var frame = new SwipeFrameLayout(Android.App.Application.Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            LeftMenu = new SwipeMenuControl(ItemWidth, SwipePlacement.Left, Android.App.Application.Context, null);
            RightMenu = new SwipeMenuControl(ItemWidth, SwipePlacement.Right, Android.App.Application.Context, null);

            frame.AddView(LeftMenu);
            frame.AddView(RightMenu);
            frame.AddView(ItemView);

            LeftMenu.Visibility = ViewStates.Gone;
            RightMenu.Visibility = ViewStates.Gone;

            LeftMenu.SetViewHolder(this);
            RightMenu.SetViewHolder(this);

            _mainItemView = ItemView;
            ItemView = frame;
            ItemView.SetOnTouchListener(this);
        }

        public SwipeMenuViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected abstract void SetupMenuAndBind();
        public bool IsMenuShown => _mainItemView.TranslationX != 0;

        public void Reset()
        {
            if (!_menuInitialized
                || (LeftMenu.Visibility == ViewStates.Gone && RightMenu.Visibility == ViewStates.Gone))
                return;

            _animator?.Cancel();
            _mainItemView.TranslationX = 0;
            ResetMenu(LeftMenu);
            ResetMenu(RightMenu);
        }

        private void ResetMenu(SwipeMenuControl swipeMenuControl)
        {
            swipeMenuControl.ResizeWidth(0);
            swipeMenuControl.SetViewHolder(this);
            swipeMenuControl.Visibility = ViewStates.Gone;
        }

        public Task HideMenusIfNeeded()
        {
            if (Math.Abs(_mainItemView.TranslationX) > MinStuckThreshold)
            {
                var taskSource = new TaskCompletionSource<bool>();
                AnimateTo(0, () => taskSource.SetResult(true));
                return taskSource.Task;
            }

            return Task.CompletedTask;
        }

        public void ShowMenu(SwipePlacement placement)
        {
            if (placement == SwipePlacement.Left)
                MoveByOffset(LeftMenu.FullSizeWidth);
            else
                MoveByOffset(RightMenu.FullSizeWidth);
        }

        public void MoveByOffset(int offsetX)
        {
            if (!_menuInitialized)
            {
                SetupMenuAndBind();
                _menuInitialized = true;
            }

            if (offsetX > 0)
            {
                RightMenu.Visibility = ViewStates.Gone;
                LeftMenu.Visibility = ViewStates.Visible;
                LeftMenu.MakeSureAllChildrenVisible();
                offsetX = LeftMenu.ResizeWidth(offsetX);
            }
            else
            {
                RightMenu.Visibility = ViewStates.Visible;
                LeftMenu.Visibility = ViewStates.Gone;
                RightMenu.MakeSureAllChildrenVisible();
                offsetX = -RightMenu.ResizeWidth(-offsetX);
            }

            _mainItemView.TranslationX = offsetX;
        }

        public override void OnAttachedToWindow()
        {
            if (_menuInitialized)
                Reset();

            base.OnAttachedToWindow();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (!IsSwipingEnabled)
                return false;

            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    TouchStarted(e.RawX);
                    break;
                case MotionEventActions.Move:
                {
                    TouchMoved(e.RawX);
                    break;
                }
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                {
                    TouchEnded(e.RawX)
                        .FireAndForget();

                    break;
                }
            }

            return _touchMoved;
        }

        private async Task TouchEnded(float x)
        {
            _touchStarted = false;
            if (Math.Abs(_mainItemView.TranslationX) > MinStuckThreshold)
            {
                float offset = x - _touchStartingPointX + _touchInitialOffsetX;
                await SnapToNearestView(offset);
            }
        }

        private void TouchMoved(float x)
        {
            if (!_touchStarted)
            {
                TouchStarted(x);
                return;
            }

            var offset = x - _touchStartingPointX;
            if (Math.Abs(offset) > MinSwipeThreshold)
            {
                SwipeStartedCommand?.Execute();
                ItemView.Parent?.RequestDisallowInterceptTouchEvent(true);
                MoveByOffset((int)(offset + _touchInitialOffsetX));
                _touchMoved = true;
            }
        }

        public IMvxCommand SwipeStartedCommand { get; set; }

        private void TouchStarted(float x)
        {
            _touchStarted = true;
            _touchStartingPointX = x;
            _touchInitialOffsetX = _mainItemView.TranslationX;
            _touchMoved = false;
            SwipeMenuAdapter.SetActiveMenu(this);
        }

        private Task SnapToNearestView(float offset)
        {
            var taskSource = new TaskCompletionSource<bool>();
            if (offset > 0)
            {
                if ((!LeftMenu.IsOpen && offset < LeftMenu.EdgeSnappingPoint)
                    || (LeftMenu.IsOpen && offset < LeftMenu.EdgeClosingPoint))
                {
                    AnimateTo(0, () => taskSource.SetResult(true));
                }
                else if (offset < LeftMenu.FullSwipeSnappingPoint
                         || !LeftMenu.FullSwipeAvailable)
                {
                    AnimateTo(LeftMenu.FullSizeWidth, () => taskSource.SetResult(true));
                }
                else
                {
                    AnimateTo(
                        ItemView.Width,
                        () =>
                        {
                            ExecuteFullSwipe(LeftMenu);
                            taskSource.SetResult(true);
                        });
                }
            }
            else
            {
                offset = -offset;
                if ((!RightMenu.IsOpen && offset < RightMenu.EdgeSnappingPoint)
                    || (RightMenu.IsOpen && offset < RightMenu.EdgeClosingPoint))
                {
                    AnimateTo(0, () => taskSource.SetResult(true));
                }
                else if (offset < RightMenu.FullSwipeSnappingPoint || !RightMenu.FullSwipeAvailable)
                {
                    AnimateTo(-RightMenu.FullSizeWidth, () => taskSource.SetResult(true));
                }
                else
                {
                    AnimateTo(
                        -ItemView.Width,
                        () =>
                        {
                            ExecuteFullSwipe(RightMenu);
                            taskSource.SetResult(true);
                        });
                }
            }

            return taskSource.Task;
        }

        private void ExecuteFullSwipe(SwipeMenuControl swipeMenuControl)
        {
            swipeMenuControl.ExecuteFullSwipe(DataContext);
            Reset();
        }

        private void AnimateTo(float destination, Action animationEnded = null)
        {
            _animator?.Cancel();
            _animator = ValueAnimator.OfFloat(_mainItemView.TranslationX, destination);
            _animator.SetDuration(AnimationDuration);
            _animator.Update += AnimatorOnUpdate;
            _animator.AnimationEnd += (sender, args) =>
            {
                _mainItemView.TranslationX = destination;
                if (destination == 0)
                {
                    LeftMenu.ResizeWidth(0);
                    RightMenu.ResizeWidth(0);
                }

                _animator = null;
                animationEnded?.Invoke();
            };
            _animator.Start();
        }

        private void AnimatorOnUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs e)
        {
            float value = (float)e.Animation.AnimatedValue;
            if (value > 0)
            {
                LeftMenu.ResizeWidth((int)value);
            }
            else
            {
                RightMenu.ResizeWidth(-(int)value);
            }

            _mainItemView.TranslationX = value;
        }
    }
}