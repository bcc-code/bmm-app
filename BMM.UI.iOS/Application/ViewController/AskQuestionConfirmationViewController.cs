using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Bindings;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;

namespace BMM.UI.iOS
{
    public partial class AskQuestionConfirmationViewController : BaseViewController<AskQuestionConfirmationViewModel>
    {
        private UIColor TextColor = new UIColor(red: 0.16f,
            green: 0.22f,
            blue: 0.04f,
            alpha: 1.00f);

        private UIColor BackgroundColor = new UIColor(red: 0.84f,
            green: 0.86f,
            blue: 0.79f,
            alpha: 1.00f);
        
        public AskQuestionConfirmationViewController() : base(null)
        {
        }

        public AskQuestionConfirmationViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Bind();
            SetThemes();
            
            var tapGestureRecognizer = new UITapGestureRecognizer(() => View!.EndEditing(true));
            tapGestureRecognizer.CancelsTouchesInView = false;
            View!.AddGestureRecognizer(tapGestureRecognizer);
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            CloseButton.Layer.BorderColor = AppColors.SeparatorColor.CGColor;
        }
        
        private void SetThemes()
        {
            View!.BackgroundColor = BackgroundColor;
            TitleLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            ThankYouLabel.ApplyTextTheme(AppTheme.Heading3);
            ThankYouLabel.TextColor = TextColor;
            DescriptionLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            DescriptionLabel.TextColor = TextColor;
            GotItButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            GotItButton.SetTitleColor(BackgroundColor, UIControlState.Normal);
            GotItButton.SetTitleColor(BackgroundColor, UIControlState.Highlighted);
            GotItButton.BackgroundColor = TextColor;

            IconImage.TintColor = TextColor;
            
            CloseButton.Layer.BorderWidth = 0.5f;
            CloseButton.Layer.ShadowRadius = 8;
            CloseButton.Layer.ShadowOffset = CGSize.Empty;
            CloseButton.Layer.ShadowOpacity = 0.1f;
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<AskQuestionConfirmationViewController, AskQuestionConfirmationViewModel>();

            set.Bind(TitleLabel)
                .To(vm => vm.TextSource[Translations.AskQuestionViewModel_Title]);
            
            set.Bind(ThankYouLabel)
                .To(vm => vm.TextSource[Translations.AskQuestionConfirmationViewModel_ThankYou]);
            
            set.Bind(DescriptionLabel)
                .To(vm => vm.TextSource[Translations.AskQuestionConfirmationViewModel_Description]);
            
            set.Bind(GotItButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.AskQuestionConfirmationViewModel_GotIt]);
            
            set.Bind(GotItButton)
                .To(vm => vm.CloseToRootCommand);
            
            set.Bind(CloseButton)
                .For(v => v.BindTap())
                .To(vm => vm.CloseToRootCommand);
            
            set.Apply();
        }
    }
}