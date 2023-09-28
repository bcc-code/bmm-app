using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using MvvmCross.Binding.Bindings.Target.Construction;

namespace BMM.UI.iOS.Bindings
{
    public class UIButtonEnabledBinding : MvxTargetBinding<UIButton, bool>
    {
        private const float EnabledAlpha = 1f;
        private const float DisabledAlpha = 0.4f;
        
        public const string BindingName = "ButtonEnabled";

        public UIButtonEnabledBinding(UIButton target)
            : base(target)
        {
        }

        protected override void SetValue(bool value)
        {
            Target.Enabled = value;
            Target.Alpha = value
                ? EnabledAlpha
                : DisabledAlpha;
        }

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public static void Register(IMvxTargetBindingFactoryRegistry registry) =>
            registry.RegisterFactory(new MvxCustomBindingFactory<UIButton>(BindingName, button => new UIButtonEnabledBinding(button)));
    }
}