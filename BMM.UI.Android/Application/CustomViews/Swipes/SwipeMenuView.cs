using _Microsoft.Android.Resource.Designer;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using BMM.Core.Constants;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views;

namespace BMM.UI.Droid.Application.CustomViews.Swipes
{
    public class SwipeMenuView :
        RelativeLayout,
        View.IOnClickListener,
        IMvxBindingContextOwner
    {
        private const int DefaultLayout = ResourceConstant.Layout.view_swipe_menu_simple;

        private object _dataContext;
        private IMvxCommand _clickCommand;
        private IMvxCommand _fullSwipeCommand;

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

        private void InflateLayout(Context context, int layoutId)
        {
            var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            BindingContext = new MvxAndroidBindingContext(context, new MvxSimpleLayoutInflaterHolder(inflater));
            inflater.Inflate(layoutId, this);
            Clickable = true;

            SetOnClickListener(this);

            TitleLabel = FindViewById<TextView>(ResourceConstant.Id.TitleLabel);
        }

        public void SetColors(Color backgroundColor)
        {
            var backgroundStateListDrawable = new StateListDrawable();

            backgroundStateListDrawable.SetExitFadeDuration(
                ViewConstants.SwiftAnimationDurationInMilliseconds);

            backgroundStateListDrawable.AddState(
                States.Default,
                new ColorDrawable(backgroundColor));

            Background = backgroundStateListDrawable;
            backgroundStateListDrawable.JumpToCurrentState();
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
        }

        public void OnClick(View v)
        {
            if (!CanExecute)
                return;

            ClickCommand?.Execute(_dataContext);
            HideMenu();
        }

        private void HideMenu()
        {
            if(Parent is SwipeMenuControl swipeMenuControl)
                swipeMenuControl.RequestHideMenu();
        }
    }
}