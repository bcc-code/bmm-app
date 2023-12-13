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
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class AskQuestionViewController : BaseViewController<AskQuestionViewModel>
    {
        private nfloat _initialYBottomViewPosition;

        public AskQuestionViewController() : base(null)
        {
        }

        public AskQuestionViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Bind();
            SetThemes();
            
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, KeyboardWillShow);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyboardWillHide);

            var tapGestureRecognizer = new UITapGestureRecognizer(() => View!.EndEditing(true));
            tapGestureRecognizer.CancelsTouchesInView = false;
            View!.AddGestureRecognizer(tapGestureRecognizer);
            
            NavigationController.PresentationController.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss
            };
            NavigationController.NavigationBarHidden = true;
        }
        
        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _initialYBottomViewPosition = BottomView.Frame.Y;
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
        }
        
        private void KeyboardWillHide(NSNotification notification)
        {
            if (BottomView != null)
            {
                double animationDuration = UIKeyboard.AnimationDurationFromNotification(notification);

                UIView.Animate(animationDuration, () =>
                {
                    BottomView.Frame = new CGRect(BottomView.Frame.X, _initialYBottomViewPosition, BottomView.Frame.Width, BottomView.Frame.Height);
                });
            }
        }

        private void KeyboardWillShow(NSNotification notification)
        {
            if (BottomView != null)
            {
                var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
                double animationDuration = UIKeyboard.AnimationDurationFromNotification(notification);
                
                UIView.Animate(animationDuration, () =>
                {
                    BottomView.Frame = new CGRect(BottomView.Frame.X, _initialYBottomViewPosition + BottomSafeArea - keyboardFrame.Height, BottomView.Frame.Width, BottomView.Frame.Height);
                });
            }
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            CloseButton.Layer.BorderColor = AppColors.SeparatorColor.CGColor;
        }
        
        private void SetThemes()
        {
            TitleLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            SubmitButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            
            CloseButton.Layer.BorderWidth = 0.5f;
            CloseButton.Layer.ShadowRadius = 8;
            CloseButton.Layer.ShadowOffset = CGSize.Empty;
            CloseButton.Layer.ShadowOpacity = 0.1f;
            
            QuestionTextView!.Font = UIFont.SystemFontOfSize(17, UIFontWeight.Medium);
            QuestionTextView!.TextColor = AppColors.LabelOneColor;
        }

        private void Bind()
        {
            var set = this.CreateBindingSet<AskQuestionViewController, AskQuestionViewModel>();

            set.Bind(TitleLabel)
                .To(vm => vm.TextSource[Translations.AskQuestionViewModel_Title]);
            
            set.Bind(SubmitButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.AskQuestionViewModel_Submit]);
            
            set.Bind(SubmitButton)
                .To(vm => vm.SubmitCommand);
            
            set.Bind(CloseButton)
                .For(v => v.BindTap())
                .To(vm => vm.CloseCommand);
            
            set.Bind(SubmitButton)
                .For(UIButtonEnabledBinding.BindingName)
                .To(vm => vm.CanSubmit);
            
            set.Bind(QuestionTextView)
                .For(v => v.Text)
                .To(vm => vm.Question);
            
            SubmitButton.Enabled = false;
            SubmitButton.Alpha = 0.4f;
            
            set.Apply();
        }
    }
}