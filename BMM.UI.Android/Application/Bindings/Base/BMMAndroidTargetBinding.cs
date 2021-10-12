using MvvmCross.Platforms.Android.Binding.Target;

namespace BMM.UI.Droid.Application.Bindings.Base
{
    public abstract class BMMAndroidTargetBinding : MvxAndroidTargetBinding
    {
        protected BMMAndroidTargetBinding(object target) : base(target)
        {
        }

        protected override object MakeSafeValue(object value) => value;
    }

    public abstract class BMMAndroidTargetBinding<TTarget, TValue>
        : MvxAndroidTargetBinding<TTarget, TValue> where TTarget : class
    {
        protected BMMAndroidTargetBinding(TTarget target) : base(target)
        {
        }

        protected override TValue MakeSafeValue(TValue value) => value;
    }
}