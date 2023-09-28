using Android.Views;
using BMM.UI.Droid.Application.Bindings.Base;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target.Construction;

public class IsEnabledBinding : BMMAndroidTargetBinding<View, bool>
{
    private const float EnabledAlpha = 1f;
    private const float DisabledAlpha = 0.4f;
    private const string BindingName = "IsEnabled";
    
    public IsEnabledBinding(View target) : base(target)
    {
    }

    protected override void SetValueImpl(View target, bool value)
    {
        target.Enabled = value;
        target.Alpha = value
            ? EnabledAlpha
            : DisabledAlpha;
    }
    
    public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;
    
    public static void Register(IMvxTargetBindingFactoryRegistry registry) =>
        registry.RegisterFactory(
            new MvxCustomBindingFactory<View>(
                BindingName,
                view => new IsEnabledBinding(view)));
}