using _Microsoft.Android.Resource.Designer;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using BMM.Core.Constants;
using BMM.Core.Models.Enums;
using FFImageLoading.Extensions;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views;

namespace BMM.UI.Droid.Application.CustomViews.Swipes
{
    public class SwipeMenuView :
        RelativeLayout,
        View.IOnClickListener,
        View.IOnLayoutChangeListener,
        IMvxBindingContextOwner
    {
        private const int DefaultLayout = ResourceConstant.Layout.view_swipe_menu;

        private readonly bool _isDefaultLayout;

        private object _dataContext;
        private IMvxCommand _clickCommand;
        private IMvxCommand _fullSwipeCommand;
        private Color _enabledBackgroundColor;
        private Color _disabledBackgroundColor;
        private Color _touchedBackgroundColor;
        private Color _separatorColor;
        private Color _iconDisabledColor;
        private Color _iconEnabledColor;
        private Color _shadowColor;

        private bool _isShadowAdded;

        private RelativeLayout _swipeContainer;
        private bool _isSeparatorCreated;
        private View _separatorView;

        private string _iconResourceName;
        private Drawable _drawable;

        protected SwipeMenuView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SwipeMenuView(Context context, int layoutId = DefaultLayout) : base(context)
        {
            InflateLayout(context, layoutId);
        }

        public SwipeMenuView(Context context) : base(context)
        {
            InflateLayout(context, DefaultLayout);
            _isDefaultLayout = true;
        }

        public SwipeMenuView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public SwipeMenuView(Context context, IAttributeSet attrs, int defStyleAttr) : base(
            context,
            attrs,
            defStyleAttr)
        {
        }

        public SwipeMenuView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(
            context,
            attrs,
            defStyleAttr,
            defStyleRes)
        {
        }

        public TextView TitleLabel { get; set; }

        public ImageView Icon { get; set; }

        public IMvxCommand ClickCommand
        {
            get => _clickCommand;
            set
            {
                _clickCommand = value;
                RefreshEnabledState();
            }
        }

        public IMvxCommand FullSwipeCommand
        {
            get => _fullSwipeCommand;
            set
            {
                _fullSwipeCommand = value;
                RefreshEnabledState();
            }
        }

        public bool ShouldAddShadow { set; get; }
        public bool ShouldAddSeparator { get; set; }

        private void InflateLayout(Context context, int layoutId)
        {
            var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            BindingContext = new MvxAndroidBindingContext(context, new MvxSimpleLayoutInflaterHolder(inflater));
            inflater.Inflate(layoutId, this);
            Clickable = true;

            SetOnClickListener(this);
            AddOnLayoutChangeListener(this);

            Icon = FindViewById<ImageView>(ResourceConstant.Id.Icon);
            TitleLabel = FindViewById<TextView>(ResourceConstant.Id.TitleLabel);
            _swipeContainer = FindViewById<RelativeLayout>(ResourceConstant.Id.SwipeContainer);
        }

        public void SetColors()
        {
            var backgroundStateListDrawable = new StateListDrawable();

            backgroundStateListDrawable.SetExitFadeDuration(
                ViewConstants.SwiftAnimationDurationInMilliseconds);

            Background = backgroundStateListDrawable;
            backgroundStateListDrawable.JumpToCurrentState();

            if (_separatorView != null)
                _separatorView.Background = new ColorDrawable(_separatorColor);
        }

        public bool Available { get; set; } = true;

        public object DataContext
        {
            get => _dataContext;
            set
            {
                _dataContext = value;
                if (BindingContext != null)
                    BindingContext.DataContext = value;
            }
        }

        public bool CanExecute => _clickCommand?.CanExecute(_dataContext) ?? false;

        public bool CanExecuteFullSwipe => _fullSwipeCommand?.CanExecute(_dataContext)
                                           ?? _clickCommand?.CanExecute(_dataContext)
                                           ?? false;

        public IMvxBindingContext BindingContext { get; set; }

        public void RefreshEnabledState()
        {
            if (Enabled == CanExecute)
                return;

            Enabled = CanExecute;

            if (Icon != null)
                Icon.Enabled = CanExecute;

            SetColors();
        }

        public void OnClick(View v)
        {
            if (!CanExecute)
                return;

            ClickCommand?.Execute(_dataContext);
            HideMenu();
        }

        public void AddShadowIfNeeded(SwipePlacement placement, int itemWidthInPx)
        {
            if (!ShouldAddShadow || _isShadowAdded)
                return;

            var gradientColors = new[]
            {
                _shadowColor,
                Color.Transparent.ToArgb()
            };

            var gradientDrawable = new GradientDrawable(
                GradientDrawable.Orientation.LeftRight,
                placement == SwipePlacement.Right
                    ? gradientColors
                    : gradientColors.Reverse().ToArray()
            );
            gradientDrawable.SetCornerRadius(0);

            var shadowView = new View(Context)
            {
                Background = gradientDrawable,
                Clickable = false
            };

            var shadowLayoutParams = new LayoutParams(
                itemWidthInPx / 2,
                ViewGroup.LayoutParams.MatchParent);

            shadowLayoutParams.AddRule(LayoutRules.AlignParentEnd);
            shadowView.LayoutParameters = shadowLayoutParams;

            AddView(shadowView);

            _isShadowAdded = true;
        }

        public void AddSeparatorIfNeeded()
        {
            if (!ShouldAddSeparator || _isSeparatorCreated)
                return;

            _separatorView = new View(Context)
            {
                Background = new ColorDrawable(_separatorColor)
            };

            var separatorLayoutParams = new LayoutParams(
                1.DpToPixels(),
                _swipeContainer.Height);

            separatorLayoutParams.AddRule(LayoutRules.AlignParentEnd);
            separatorLayoutParams.AddRule(LayoutRules.CenterVertical);
            _separatorView.LayoutParameters = separatorLayoutParams;
            _swipeContainer.AddView(_separatorView);

            _isSeparatorCreated = true;
        }

        public void OnLayoutChange(
            View v,
            int left,
            int top,
            int right,
            int bottom,
            int oldLeft,
            int oldTop,
            int oldRight,
            int oldBottom)
        {
            int expectedSeparatorHeight = _isDefaultLayout
                ? _swipeContainer.Height
                : _swipeContainer.Height - 2 * 10.DpToPixels();

            if (_separatorView == null || _separatorView.Height == expectedSeparatorHeight)
                return;

            var separatorViewLayoutParams = _separatorView.LayoutParameters;
            separatorViewLayoutParams.Height = expectedSeparatorHeight;
            _separatorView.LayoutParameters = separatorViewLayoutParams;
        }
        

        private void HideMenu()
        {
            if(Parent is SwipeMenuControl swipeMenuControl)
                swipeMenuControl.RequestHideMenu();
        }
    }
}