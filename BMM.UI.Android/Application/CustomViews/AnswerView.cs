using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using BMM.Api.Implementation.Models;
using BMM.UI.Droid.Application.Bindings;
using BMM.UI.Droid.Application.Extensions;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.Droid.Application.CustomViews
{
    [Register("bmm.ui.droid.application.customViews.AnswerView")]
    public class AnswerView
        : FrameLayout,
          IMvxBindingContextOwner
    {
        private readonly Action<Answer> _clickedAction;
        private LinearLayout _backgroundView;

        protected AnswerView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public AnswerView(Context context, Action<Answer> clickedAction)
            : base(context)
        {
            _clickedAction = clickedAction;
            Initialize(context);
        }

        public AnswerView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(context);
        }

        public AnswerView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }

        public IMvxBindingContext BindingContext { get; set; }

        private void Initialize(Context context)
        {
            this.LayoutAndAttachBindingContextOrDesignMode(
                context,
                Resource.Layout.view_answer);
            
            SetBackgroundColor(Color.Transparent);
            _backgroundView = FindViewById<LinearLayout>(Resource.Id.BackgroundView);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            _clickedAction?.Invoke(BindingContext.DataContext as Answer);
            return base.OnTouchEvent(e);
        }

        private void ApplyBounceAnimation(View view)
        {
            var bounceAnimator = ObjectAnimator.OfFloat(view, "translationY", 0f, -40f, 0f);
            bounceAnimator.SetDuration(500);
            bounceAnimator.SetInterpolator(new OvershootInterpolator(1f));
            bounceAnimator.Start();
        }
        
        private void ShakeAnimation(View view)
        {
            Animation anim = new TranslateAnimation(-5,5,0,0);
            anim.Duration = 100;
            anim.RepeatMode = RepeatMode.Reverse;
            anim.RepeatCount = 5;
            view.StartAnimation(anim);
            
            _backgroundView.SetBackgroundColor(Context.GetColorFromResource(Resource.Color.global_white_three));
        }
    }
}