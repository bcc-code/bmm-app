using System;
using Android.Content;
using Android.Util;
using AndroidX.CardView.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace BMM.UI.Droid.Application.Bindings
{
    /// <summary>
    /// If the value is true, it transforms it into a circular / completely round image.
    /// This has been implemented to implement the requirements for podcasts and contributors. However it falls short in other use cases.
    /// E.g. it won't change back to non-circular if the value property changes to false.
    /// Also if the CardView is really big it might only get rounded corners (I can't image such a big CardView though).
    /// Therefore be cautious when using this.
    /// </summary>
    public class MvxCardCircle : MvxAndroidTargetBinding
    {
        public MvxCardCircle(object target) : base(target)
        { }

        public override Type TargetType => typeof(CardView);

        protected override void SetValueImpl(object target, object value)
        {
            var card = (CardView)target;
            if (value is bool test && test)
            {
                int imageHeightInDp = 160; // should be the same value as in listitem_tracklist_header.xml
                var pixel = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, imageHeightInDp, AndroidGlobals.ApplicationContext.Resources.DisplayMetrics) / 2;
                card.Radius = pixel;
            }
            // Caution: this does not work if it changes dynamically
        }
    }
}