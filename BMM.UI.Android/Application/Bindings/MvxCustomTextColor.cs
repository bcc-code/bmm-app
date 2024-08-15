using Android.Graphics;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Plugin.Color.Platforms.Android.BindingTargets;

namespace BMM.UI.Droid.Application.Bindings;

/// <summary>
/// For some reason TextColor from MvvmCross.Plugin.Color is not working on Android.
/// Therefore we're using this binding as a workaround.
/// </summary>
public class MvxCustomTextColor : MvxViewColorBinding
{
    public MvxCustomTextColor(TextView view): base(view)
    {
    }

    protected override void SetValueImpl(object target, object value)
    {
        var view = (TextView)target;
        view.SetTextColor((Color) value);
    }

    public static void Register(IMvxTargetBindingFactoryRegistry registry)
    {
        registry.RegisterFactory(new MvxCustomBindingFactory<TextView>("CustomTextColor", textView => new MvxCustomTextColor(textView)));
    }
}