using Android.Content;
using Android.Views;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Views;

namespace BMM.UI.Droid.Application.Bindings
{
    public static class BindingExtensions
    {
        public static void LayoutAndAttachBindingContextOrDesignMode(
            this IMvxBindingContextOwner bindingContextOwner,
            Context context,
            int layoutId,
            Action onAttachedAction = null,
            bool copyDataContext = false)
        {
            if (context is IMvxBindingContextOwner)
            {
                SetBindingContext(context, bindingContextOwner, (IMvxLayoutInflaterHolder)context, copyDataContext);
                Inflate(bindingContextOwner, layoutId);
                onAttachedAction?.Invoke();
            }
            else if (context is ContextThemeWrapper)
            {
                SetBindingContext(
                    context,
                    bindingContextOwner,
                    new MvxSimpleLayoutInflaterHolder(Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity.LayoutInflater),
                    copyDataContext);
                Inflate(bindingContextOwner, layoutId);
                onAttachedAction?.Invoke();
            }
            else
            {
                View.Inflate(context, layoutId, (ViewGroup)bindingContextOwner);
            }
        }

        private static void Inflate(IMvxBindingContextOwner bindingContextOwner, int layoutId)
            => ((IMvxAndroidBindingContext)bindingContextOwner.BindingContext).BindingInflate(
                layoutId,
                (ViewGroup)bindingContextOwner);

        private static void SetBindingContext(
            Context context,
            IMvxBindingContextOwner bindingContextOwner,
            IMvxLayoutInflaterHolder mvxLayoutInflaterHolder,
            bool copyDataContext)
        {
            bindingContextOwner.BindingContext = new MvxAndroidBindingContext(context, mvxLayoutInflaterHolder);
            if (copyDataContext && context is IMvxBindingContextOwner parentBindingContextOwner)
                bindingContextOwner.BindingContext.DataContext = parentBindingContextOwner.BindingContext.DataContext;
        }
    }
}