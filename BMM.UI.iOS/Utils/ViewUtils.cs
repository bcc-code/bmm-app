using System;
using UIKit;

namespace BMM.UI.iOS.Utils
{
    public class ViewUtils
    {
        public static void RunAnimation(float duration, Action action)
        {
            UIView.Animate(
                duration,
                0,
                UIViewAnimationOptions.AllowUserInteraction,
                () => action(),
                null);
        }
    }
}