using Android.Views;
using Android.Widget;
using BMM.UI.Droid.Application.Bindings.Base;
using BMM.UI.Droid.Application.Constants;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target.Construction;

namespace BMM.UI.Droid.Application.Bindings
{
    public class ImageButtonIconResourceBinding : BMMAndroidTargetBinding<ImageButton, int>
    {
        private const string BindingName = "IconResource";

        public ImageButtonIconResourceBinding(ImageButton target) : base(target)
        {
        }

        protected override void SetValueImpl(ImageButton target, int resourceId)
        {
            target.SetImageResource(resourceId);
        }

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        public static void Register(IMvxTargetBindingFactoryRegistry registry) =>
            registry.RegisterFactory(
                new MvxCustomBindingFactory<ImageButton>(
                    BindingName,
                    view => new ImageButtonIconResourceBinding(view)));
    }
}