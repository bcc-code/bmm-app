using Android.Views;
using MvvmCross.Plugin.Color.Platforms.Android.BindingTargets;

namespace BMM.UI.Droid.Application.Bindings
{
    public class MvxBackgroundTintBinding : MvxViewColorBinding
    {
        public MvxBackgroundTintBinding(View view) : base(view)
        { }

        protected override void SetValueImpl(object target, object value)
        {
            var view = (View)target;
            var colorString = value as string;
            view.BackgroundTintList = ColorStateListHelper.ParseString(colorString);
        }
    }
}