using BMM.Core.Translation;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Bindings;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Views;

namespace BMM.UI.iOS
{
    public partial class SuggestEditViewController : BaseViewController<SuggestEditViewModel>
    {
        private nfloat _initialYBottomViewPosition;

        public SuggestEditViewController() : base(null)
        {
        }

        public SuggestEditViewController(string nib) : base(nib)
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
                
                TranscriptionsTableView.ContentInset = UIEdgeInsets.Zero;
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
                
                var firstResponder = TranscriptionsTableView.FindFirstResponder();
                if (firstResponder == null)
                    return;

                var positionInWindow = firstResponder.Superview.ConvertPointToView(firstResponder.Frame.Location, null);
                var viewBottom = positionInWindow.Y + firstResponder.Frame.Height;
                
                if (viewBottom < keyboardFrame.GetMinY())
                    return;
                
                var contentInset = TranscriptionsTableView.ContentInset;
                contentInset.Bottom = keyboardFrame.Height;
                TranscriptionsTableView.ContentInset = contentInset;
            }
        }

        private void SetThemes()
        {
            TitleLabel.ApplyTextTheme(AppTheme.Title1);
            SubtitleLabel.ApplyTextTheme(AppTheme.Subtitle2Label3);
            SubmitButton.ApplyButtonStyle(AppTheme.ButtonPrimary);
            CancelButton.ApplyButtonStyle(AppTheme.ButtonSecondaryMedium);
        }

        private void Bind()
        {
            var source = new BaseSimpleTableViewSource(TranscriptionsTableView, SuggestEditTableViewCell.Key);
            
            var set = this.CreateBindingSet<SuggestEditViewController, SuggestEditViewModel>();
            
            set.Bind(source)
                .To(vm => vm.Transcriptions);

            set.Bind(TitleLabel)
                .To(vm => vm.TextSource[Translations.SuggestEditViewModel_Title]);
            
            set.Bind(SubtitleLabel)
                .To(vm => vm.TextSource[Translations.SuggestEditViewModel_Subtitle]);
            
            set.Bind(SubmitButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.SuggestEditViewModel_Submit]);
            
            set.Bind(SubmitButton)
                .To(vm => vm.SubmitCommand);
            
            set.Bind(SubmitButton)
                .For(UIButtonEnabledBinding.BindingName)
                .To(vm => vm.CanSubmit);
            
            set.Bind(CancelButton)
                .For(v => v.BindTitle())
                .To(vm => vm.TextSource[Translations.SuggestEditViewModel_Cancel]);

            set.Bind(CancelButton)
                .To(vm => vm.CloseCommand);
            
            SubmitButton.Enabled = false;
            SubmitButton.Alpha = 0.4f;
            
            set.Apply();
        }
    }
}