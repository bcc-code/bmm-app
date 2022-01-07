using Android.Views;
using BMM.Core.Constants;
using BMM.UI.Droid.Application.Bindings.Base;
using BMM.UI.Droid.Application.Constants;
using Java.Lang;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target.Construction;

namespace BMM.UI.Droid.Application.Bindings
{
    public class BackgroundResourceBinding : BMMAndroidTargetBinding<View, int>
    {
        private const string BindingName = "BackgroundResource";

        public BackgroundResourceBinding(View target) : base(target)
        {
        }

        protected override void SetValueImpl(View target, int resourceId)
        {
            if (resourceId == ValueConstants.None)
            {
                target.Background = null;
                return;
            }

            target.SetBackgroundResource(resourceId);
        }

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        public static void Register(IMvxTargetBindingFactoryRegistry registry) =>
            registry.RegisterFactory(
                new MvxCustomBindingFactory<View>(
                    BindingName,
                    view => new BackgroundResourceBinding(view)));
    }
}