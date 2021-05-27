using System;
using AndroidX.CardView.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace BMM.UI.Droid.Application.Bindings
{
    public class MvxCardCircle : MvxAndroidTargetBinding
    {
        public MvxCardCircle(object target) : base(target)
        { }

        public override Type TargetType => typeof(CardView);

        protected override void SetValueImpl(object target, object value)
        {
            var card = (CardView)target;
            if (value is bool test && test)
                card.Radius = 10000; // this takes pixel whereas most other units are in dp
            // Caution: this does not work if it changes dynamically
        }
    }
}