using System;
using BMM.Core.Constants;
using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS.Extensions
{
    public static class ViewExtensions
    {
        /// <summary>
        /// This method should be used for changing Hidden state in views added to UIStackView
        /// as this property needs to be guarded of multiple set the same state.
        /// </summary>
        public static void SetHiddenIfNeeded(this UIView view, bool isHidden)
        {
            if (view.Hidden == isHidden)
                return;

            view.Hidden = isHidden;
        }
        
        public static void RunAnimation(this UIView view, float duration, Action action)
        {
            view.InvokeOnMainThread(() =>
            {
                UIView.Animate(
                    duration,
                    0,
                    UIViewAnimationOptions.AllowUserInteraction,
                    action,
                    null);
            });
        }
    }
}