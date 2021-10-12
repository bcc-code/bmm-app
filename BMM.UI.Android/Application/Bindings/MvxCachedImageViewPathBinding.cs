using Android.Views;
using BMM.UI.Droid.Application.Bindings.Base;
using FFImageLoading.Cross;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target.Construction;

namespace BMM.UI.Droid.Application.Bindings
{
    public class MvxCachedImageViewPathBinding : BMMAndroidTargetBinding<MvxCachedImageView, string>
    {
        private const string BindingName = "ImageViewPath";

        public MvxCachedImageViewPathBinding(MvxCachedImageView target) : base(target)
        {
        }

        protected override void SetValueImpl(MvxCachedImageView target, string value)
        {
            if (target.ImagePath == value)
                return;

            target.ImagePath = value;
        }

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        public static void Register(IMvxTargetBindingFactoryRegistry registry) =>
            registry.RegisterFactory(
                new MvxCustomBindingFactory<MvxCachedImageView>(
                    BindingName,
                    view => new MvxCachedImageViewPathBinding(view)));
    }
}