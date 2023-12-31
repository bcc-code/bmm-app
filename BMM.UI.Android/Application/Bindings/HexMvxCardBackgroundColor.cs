using Android.Content.Res;
using AndroidX.CardView.Widget;
using BMM.UI.Droid.Application.Bindings.Base;
using Microsoft.Maui.Graphics.Platform;
using MvvmCross.Binding.Bindings.Target.Construction;

namespace BMM.UI.Droid.Application.Bindings
{
    public class HexMvxCardBackgroundColor : BMMAndroidTargetBinding<CardView, string>
    {
        public HexMvxCardBackgroundColor(CardView target) : base(target)
        {
        }

        protected override void SetValueImpl(CardView target, string value)
        {
            var color = Microsoft.Maui.Graphics.Color.FromArgb(value);
            Target.CardBackgroundColor = ColorStateList.ValueOf(color.AsColor());
        }
        
        public static void Register(IMvxTargetBindingFactoryRegistry registry) =>
            registry.RegisterFactory(
                new MvxCustomBindingFactory<CardView>(
                    nameof(HexMvxCardBackgroundColor),
                    view => new HexMvxCardBackgroundColor(view)));
    }
}