using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using AndroidX.ConstraintLayout.Widget;
using BMM.Core.Models.Parameters;
using BMM.UI.Droid.Application.Bindings;
using MvvmCross.Binding.BindingContext;

namespace BMM.UI.Droid.Application.CustomViews
{
    [Register("bmm.ui.droid.application.customViews.BmmDialog")]
    public class BmmDialog
        : FrameLayout,
          IMvxBindingContextOwner
    {
        private ConstraintLayout _dialogPopupLayout;

        protected BmmDialog(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public BmmDialog(Context context)
            : base(context)
        {
            Initialize(context);
        }

        public BmmDialog(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(context);
        }

        public BmmDialog(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Initialize(context);
        }

        public IMvxBindingContext BindingContext { get; set; }

        private void CreateBinding()
        {
            var set = this.CreateBindingSet<BmmDialog, IDialogParameter>();

            var headerLabel = FindViewById<TextView>(Resource.Id.HeaderLabel);
            var titleLabel = FindViewById<TextView>(Resource.Id.TitleLabel);
            var subtitleLabel = FindViewById<TextView>(Resource.Id.Subtitle);
            var closeButton = FindViewById<Button>(Resource.Id.CloseButton);
            
            set.Bind(headerLabel)
                .To(p => p.Header);

            set.Bind(titleLabel)
                .To(p => p.Title);

            set.Bind(subtitleLabel)
                .To(p => p.Subtitle);

            set.Bind(closeButton)
                .For(v => v.Text)
                .To(p => p.CloseButtonText);
            
            set.Bind(closeButton)
                .For(v => v.Text)
                .To(p => p.CloseButtonText);
            
            set.Bind(closeButton)
                .To(p => p.CloseCommand);
            
            set.Apply();
        }

        private void Initialize(Context context)
        {
            this.LayoutAndAttachBindingContextOrDesignMode(
                context,
                Resource.Layout.view_bmm_dialog,
                () => this.DelayBind(CreateBinding));
            
            _dialogPopupLayout = FindViewById<ConstraintLayout>(Resource.Id.DialogPopupLayout);
        }
    }
}