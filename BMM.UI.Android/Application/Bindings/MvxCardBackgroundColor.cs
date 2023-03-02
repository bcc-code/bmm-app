using System;
using Android.Graphics;
using AndroidX.CardView.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace BMM.UI.Droid.Application.Bindings
{
    public class MvxCardBackgroundColor : MvxAndroidTargetBinding {
        public MvxCardBackgroundColor(object target) : base(target)
        { }

        public override Type TargetValueType => typeof(Color);

        protected override void SetValueImpl(object target, object value)
        {
            var card = (CardView)target;
            card.CardBackgroundColor = ColorStateListHelper.ParseString(value as string);
        }
    }
}