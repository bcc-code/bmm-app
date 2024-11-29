using BMM.Core.Extensions;
using BMM.UI.iOS.Enums;
using BMM.UI.iOS.TableViewSource.Base;
using MvvmCross.Commands;
using static BMM.Core.Constants.ViewConstants;

namespace BMM.UI.iOS.TableViewCell.Base
{
    public abstract class SwipeableViewCell : BaseBMMTableViewCell
    {
        public UISwipeMenu LeftMenu { get; private set; }
        public UISwipeMenu RightMenu { get; private set; }
        private NSLayoutConstraint _leftConstraint;
        private NSLayoutConstraint _rightConstraint;
        private nfloat _leftConstant;
        private nfloat _rightConstant;
        private UIView _mainView;
        private readonly UIPanGestureRecognizer _panGestureRecognizer;
        private float _startingTouchOffsetLeft;
        private float _startingTouchOffsetRight;
        private bool _initialized;
        private ISwipeableTableViewSource _swipeableSource;

        protected SwipeableViewCell(IntPtr handle) : base(handle)
        {
            LeftMenu = new UISwipeMenu(SwipePlacement.Left, this);
            RightMenu = new UISwipeMenu(SwipePlacement.Right, this);

            _panGestureRecognizer = new PanDirectionGestureRecognizer(PanDirection.Horizontal);
            _panGestureRecognizer.AddTarget(HandleMove);
        }

        public bool Enabled { get => _panGestureRecognizer.Enabled; set => _panGestureRecognizer.Enabled = value; }
        public bool IsBeingTouched { get; set; }
        public IMvxCommand SwipeStartedCommand { get; set; }

        public void SetRecognizerDelegate(UIGestureRecognizerDelegate gestureDelegate)
        {
            _panGestureRecognizer.Delegate = gestureDelegate;
        }

        public void SetSwipeableSource(ISwipeableTableViewSource swipeableSource)
        {
            _swipeableSource = swipeableSource;
        }

        public abstract void SetupAndBindMenus();

        protected void AttachToCenterView()
        {
            ContentView.Add(LeftMenu);
            ContentView.Add(RightMenu);
            ContentView.AddGestureRecognizer(_panGestureRecognizer);

            _mainView = ContentView.Subviews.FirstOrDefault();
            if (_mainView != null)
            {
                _leftConstraint =
                    ContentView.Constraints.First(x => x.FirstAttribute == NSLayoutAttribute.Leading && x.Active);
                _leftConstant = _leftConstraint.Constant;
                _rightConstraint =
                    ContentView.Constraints.First(x => x.FirstAttribute == NSLayoutAttribute.Trailing && x.Active);
                _rightConstant = _rightConstraint.Constant;
            }
        }

        private void HandleMove()
        {
            if (_swipeableSource?.CellWithSwipeInProgress != null &&
                    _swipeableSource?.CellWithSwipeInProgress != this)
            {
                return;
            }

            var offset = _panGestureRecognizer.TranslationInView(this);

            switch (_panGestureRecognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    HandleTouchBegan();
                    break;
                case UIGestureRecognizerState.Changed:
                    UpdateSwipeFromTouch(offset.X);
                    break;
                case UIGestureRecognizerState.Ended:
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    HandleTouchEnded(offset.X)
                        .FireAndForget();
                    break;
            }
        }

        public async Task PresentSwipe()
        {
            var taskSource = new TaskCompletionSource<bool>();
            InvokeOnMainThread(async () => await PresentSwipe(taskSource));
            await taskSource.Task;

            async Task PresentSwipe(TaskCompletionSource<bool> taskCompletionSource)
            {
                HandleTouchBegan();
                float x = RightMenu.GetPointToSnap(RightMenu.ItemWidth);
                await HandleTouchEnded(x);
                await Task.Delay(TimeSpan.FromSeconds(LongAnimationDuration));

                AnimateResetSwipe(
                    async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(LongAnimationDuration));
                        HandleTouchBegan();
                        x = LeftMenu.GetPointToSnap(LeftMenu.ItemWidth);
                        await HandleTouchEnded(x);
                        taskCompletionSource.TrySetResult(true);
                    });
            }
        }

        private async Task HandleTouchEnded(nfloat x)
        {
            if (!IsBeingTouched)
                return;

            await SnapToNearestView(x);

            if (Superview is UITableView tableView)
            {
                tableView.ScrollEnabled = true;
            }

            IsBeingTouched = false;
            if (_swipeableSource != null)
            {
                _swipeableSource.CellWithSwipeInProgress = null;
            }
        }

        private void HandleTouchBegan()
        {
            if (_swipeableSource != null)
            {
                _swipeableSource.CellWithSwipeInProgress = this;
            }
            InitializeIfNeeded();
            SwipeStartedCommand?.Execute();

            if (Superview is UITableView tableView)
                tableView.ScrollEnabled = false;

            IsBeingTouched = true;
            _swipeableSource?.ResetVisibleCells(true);
            RefreshMenu(LeftMenu);
            RefreshMenu(RightMenu);

            _startingTouchOffsetLeft = (float)(_leftConstraint.Constant - _leftConstant);
            _startingTouchOffsetRight = (float)(_rightConstraint.Constant - _rightConstant);
        }

        private void RefreshMenu(UISwipeMenu menu)
        {
            menu.UpdateAvailability();
        }

        private void InitializeIfNeeded()
        {
            if (!_initialized)
            {
                SetupAndBindMenus();
                LayoutIfNeeded();
                _initialized = true;
            }
        }

        public Task AnimateToOffset(
            nfloat offsetX,
            float duration = DefaultAnimationDuration,
            Func<Task> onAnimationEnd = null)
        {
            var taskSource = new TaskCompletionSource<bool>();

            LayoutIfNeeded();
            Animate(
                duration,
                () =>
                {
                    UpdateSwipe(offsetX);
                },
                async () =>
                {
                    if (onAnimationEnd != null)
                        await onAnimationEnd.Invoke();
                    taskSource.TrySetResult(true);
                });

            return taskSource.Task;
        }

        private async Task SnapToNearestView(nfloat offsetX)
        {
            var delta = _startingTouchOffsetLeft + offsetX;
            if (delta > 0)
            {
                //left menu
                float point = LeftMenu.GetPointToSnap(delta);

                if (point > Frame.Width)
                {
                    await AnimateToOffset(
                        point,
                        DefaultAnimationDuration,
                        () => ExecuteCommandAndReset(LeftMenu));
                }
                else
                {
                    await AnimateToOffset(point);
                }
            }
            else
            {
                //right menu
                float point = RightMenu.GetPointToSnap(delta);

                if (point < -Frame.Width)
                {
                    await AnimateToOffset(
                        point,
                        DefaultAnimationDuration,
                        () => ExecuteCommandAndReset(RightMenu));
                }
                else
                {
                    await AnimateToOffset(point);
                }
            }
        }

        private async Task ExecuteCommandAndReset(UISwipeMenu menu)
        {
            await menu.ExecuteFullSwipe(DataContext);
            Animate(
                DefaultAnimationDuration,
                0.2d,
                UIViewAnimationOptions.CurveEaseInOut,
                Reset,
                null);
        }

        public void Refresh()
        {
            RefreshMenu(LeftMenu);
            RefreshMenu(RightMenu);
        }

        public void ResetSwipe()
        {
            if (IsResetNeeded())
            {
                Reset();
            }
        }

        private void Reset()
        {
            _startingTouchOffsetLeft = 0;
            _startingTouchOffsetRight = 0;
            UpdateSwipe(0);
        }

        public void AnimateResetSwipe(Action onCompletion = null)
        {
            if (IsResetNeeded())
            {
                LayoutIfNeeded();
                Animate(
                    DefaultAnimationDuration,
                    Reset,
                    () => onCompletion?.Invoke());
            }
        }

        private bool IsResetNeeded()
        {
            return _initialized
                   && !IsBeingTouched
                   && (_rightConstraint.Constant != _rightConstant || _rightConstraint.Constant != _rightConstant);
        }

        private void UpdateSwipe(nfloat delta)
        {
            _leftConstraint.Constant = _leftConstant + delta;
            _rightConstraint.Constant = _rightConstant - delta;
            Superview?.LayoutIfNeeded();
        }

        private void UpdateSwipeFromTouch(nfloat delta)
        {
            if (!IsBeingTouched)
                return;

            var leftOffset = _startingTouchOffsetLeft + delta;
            var rightOffset = _startingTouchOffsetRight - delta;

            if (LeftMenu.MaxSwipePoint < leftOffset || RightMenu.MaxSwipePoint < rightOffset)
                return;

            _leftConstraint.Constant = leftOffset + _leftConstant;
            _rightConstraint.Constant = rightOffset + _rightConstant;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            AttachToCenterView();
        }
    }
}