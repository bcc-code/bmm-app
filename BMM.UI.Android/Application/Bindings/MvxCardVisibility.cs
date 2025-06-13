using System;
using Android.Views;
using AndroidX.CardView.Widget;
using MvvmCross.Platforms.Android.Binding.Target;

namespace BMM.UI.Droid.Application.Bindings
{
    public class MvxCardVisibility : MvxAndroidTargetBinding {
        public MvxCardVisibility(object target) : base(target)
        { }

        public override Type TargetValueType => typeof(CardView);

        protected override void SetValueImpl(object target, object value)
        {
            var card = (CardView)target;
            bool? boolean = value is bool ? (bool)value : false;
            card.Visibility = boolean == false ? ViewStates.Gone : ViewStates.Visible;
        }
    }
}