using System.ComponentModel;
using System.Drawing;
using BMM.Core.Constants;
using BMM.Core.Models.Parameters;
using BMM.UI.iOS.Constants;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;
using ObjCRuntime;

namespace BMM.UI.iOS.CustomViews
{
    [DesignTimeVisible(true)]
    public partial class BmmDialog : MvxView
    {
        public BmmDialog(CGRect bounds)
            : base(new RectangleF((float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height))
        {
            Initialize();
        }

        public BmmDialog(ObjCRuntime.NativeHandle handle) : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            XibLoad();
            this.CreateBindingContext();
            this.DelayBind(Bind);
        }

        private void XibLoad()
        {
            var nibsArray = NSBundle.MainBundle.LoadNib(nameof(BmmDialog), this, null);
            var view = Runtime.GetNSObject<UIView>(nibsArray.ValueAt(0));
            view.Frame = Bounds;
            AddSubview(view);
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<BmmDialog, IDialogParameter>();

            set.Bind(HeaderLabel)
                .To(p => p.Header);

            set.Bind(TitleLabel)
                .To(p => p.Title);

            set.Bind(SubtitleLabel)
                .To(p => p.Subtitle);

            set.Bind(CloseButton)
                .For(v => v.BindTitle())
                .To(p => p.CloseButtonText);
            
            CloseButton.TouchUpInside += CloseButtonOnTouchUpInside;
            BackgroundColor = UIColor.Black.ColorWithAlpha(0.6f);
            ContainerView.Layer.CornerRadius = 24;
            
            HeaderLabel.ApplyTextTheme(AppTheme.Title1);
            HeaderLabel.TextColor = AppColors.UtilityAutoColor;
            TitleLabel.ApplyTextTheme(AppTheme.Title1);
            SubtitleLabel.ApplyTextTheme(AppTheme.Subtitle1Label2);
            SetConstraints();
                
            set.Apply();
        }

        private void CloseButtonOnTouchUpInside(object sender, EventArgs e)
        {
            Animate(
                ViewConstants.DefaultAnimationDuration,
                () => Alpha = ViewConstants.InvisibleAlpha,
                completion: RemoveFromSuperview);
        }

        private void SetConstraints()
        {
            SetPopupConstraints();
        }

        private void SetPopupConstraints()
        {
            PopupView.TranslatesAutoresizingMaskIntoConstraints = false;

            PopupView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            PopupView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            PopupView.WidthAnchor.ConstraintEqualTo(314).Active = true;
        }
    }
}