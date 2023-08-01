using Android.Views;
using BMM.UI.Droid.Application.Bindings.Base;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target.Construction;

public class AlphaTargetBinding : BMMAndroidTargetBinding<View, float>
{
    private const string BindingName = "ViewAlpha";
    
    public AlphaTargetBinding(View target) : base(target)
    {
    }

    protected override void SetValueImpl(View target, float value)
    {
        target.Alpha = value;
    }
    
    public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;
    
    public static void Register(IMvxTargetBindingFactoryRegistry registry) =>
        registry.RegisterFactory(
            new MvxCustomBindingFactory<View>(
                BindingName,
                view => new AlphaTargetBinding(view)));
}