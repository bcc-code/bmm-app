using System;
using Android.Animation;

namespace BMM.UI.Droid.Application.Listeners
{
    public class AnimationUpdateListener
        : Java.Lang.Object,
          ValueAnimator.IAnimatorUpdateListener
    {
        private readonly Action<ValueAnimator> _onAnimationUpdateAction;

        public AnimationUpdateListener(Action<ValueAnimator> onAnimationUpdateAction)
        {
            _onAnimationUpdateAction = onAnimationUpdateAction;
        }
        
        public void OnAnimationUpdate(ValueAnimator animation) => _onAnimationUpdateAction?.Invoke(animation);
    }
}