using System.Drawing;
using BMM.Core.Extensions;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.TableViewCell.Base;
using MvvmCross.Commands;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.WeakSubscription;

namespace BMM.UI.iOS.CustomViews.Swipes.Base
{
    public abstract class SwipeMenuBase : MvxView, IUIGestureRecognizerDelegate
    {
        private MvxCanExecuteChangedEventSubscription _canExecuteClickCommandSubscription;
        private MvxCanExecuteChangedEventSubscription _canExecuteFullSwipeCommandSubscription;
        private IMvxCommand _clickCommand;
        private IMvxCommand _fullSwipeCommand;
        private bool _initialized;
        private UITapGestureRecognizer _tapGestureRecognizer;
        private UILongPressGestureRecognizer _longPressGestureRecognizer;

        protected SwipeMenuBase(RectangleF bounds) : base(bounds)
        {
        }

        protected SwipeMenuBase(IntPtr handle) : base(handle)
        {
        }

        protected SwipeMenuBase(CGRect handle) : base(handle)
        {
        }

        protected SwipeMenuBase()
        {
        }

        public abstract UILabel LabelTitle { get; }
        public abstract NSLayoutConstraint ConstraintSeparatorLeading { get; }
        public abstract NSLayoutConstraint ConstraintSeparatorHeight { get; }
        public abstract UIView ContainerView { get; }
        public abstract int SeparatorVerticalMargin { get; }
        public bool Available { get; set; } = true;
        public bool TreatAsSingleAction { set; get; }
        public bool ShouldAddShadow { set; get; }
        public bool ShouldAddSeparator { get; set; }
        public bool CanExecute => _clickCommand?.CanExecute(DataContext) ?? false;

        public bool CanExecuteFullSwipe => _fullSwipeCommand?.CanExecute(DataContext)
                                           ?? _clickCommand?.CanExecute(DataContext)
                                           ?? false;

        public IMvxCommand ClickCommand
        {
            get => _clickCommand;
            set
            {
                _canExecuteClickCommandSubscription?.Dispose();
                _canExecuteClickCommandSubscription = null;

                if (value != null)
                {
                    //_canExecuteClickCommandSubscription = value.WeakSubscribe((obj, args) => SetColors());
                    _clickCommand = value;
                }
            }
        }

        public IMvxCommand FullSwipeCommand
        {
            get => _fullSwipeCommand;
            set
            {
                _canExecuteFullSwipeCommandSubscription?.Dispose();
                _canExecuteFullSwipeCommandSubscription = null;

                if (value != null)
                {
                    //_canExecuteFullSwipeCommandSubscription = value.WeakSubscribe((obj, args) => SetColors());
                    _fullSwipeCommand = value;
                }
            }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if(_initialized)
                return;

            _tapGestureRecognizer = new UITapGestureRecognizer
            {
                Delegate = this
            };

           _tapGestureRecognizer.AddTarget(ClickAction);
           AddGestureRecognizer(_tapGestureRecognizer);

           _initialized = true;
           SetThemes();
        }

        private void SetThemes()
        {
            LabelTitle.ApplyTextTheme(AppTheme.Subtitle2Label1);
            LabelTitle.TextColor = AppColors.GlobalWhiteOneColor;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _canExecuteClickCommandSubscription?.Dispose();
                _canExecuteClickCommandSubscription = null;
                _canExecuteFullSwipeCommandSubscription?.Dispose();
                _canExecuteFullSwipeCommandSubscription = null;
                _tapGestureRecognizer?.Dispose();
                _tapGestureRecognizer = null;
                _longPressGestureRecognizer?.Dispose();
                _longPressGestureRecognizer = null;
            }

            base.Dispose(isDisposing);
        }

        private void ClickAction()
        {
            if (!CanExecute)
                return;

            ClickCommand.Execute(DataContext);
            HideMenu();
        }

        private void HideMenu()
        {
            if (Superview is UISwipeMenu swipeMenu)
                swipeMenu.RequestHideMenus();
        }

        [Export("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
        public bool ShouldRecognizeSimultaneously(
            UIGestureRecognizer gestureRecognizer,
            UIGestureRecognizer otherGestureRecognizer)
        {
            return true;
        }
    }
}