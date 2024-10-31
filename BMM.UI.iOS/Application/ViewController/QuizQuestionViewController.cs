using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Models.POs.Question.Interfaces;
using BMM.Core.ViewModels;
using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews;
using BMM.UI.iOS.CustomViews.Enums;
using BMM.UI.iOS.Extensions;
using CoreAnimation;
using FFImageLoading.Cross;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views.Gestures;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

namespace BMM.UI.iOS
{
    [MvxModalPresentation(WrapInNavigationController = true, ModalPresentationStyle = UIModalPresentationStyle.PageSheet)]
    public partial class QuizQuestionViewController : BaseViewController<QuizQuestionViewModel>
    {
        private const string Fade = "fade";
        private IQuestionPO _questionPO;

        public QuizQuestionViewController() : base(null)
        {
        }

        public QuizQuestionViewController(string nib) : base(nib)
        {
        }

        public override Type ParentViewControllerType => null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var set = this.CreateBindingSet<QuizQuestionViewController, QuizQuestionViewModel>();

            set.Bind(CloseIconView)
                .For(v => v.BindTap())
                .To(vm => vm.CloseCommand);

            set.Bind(this)
                .For(v => v.QuestionPO)
                .To(vm => vm.QuestionPO);
            
            set.Apply();

            NavigationController!.NavigationBarHidden = true;
            NavigationController!.PresentationController!.Delegate = new CustomUIAdaptivePresentationControllerDelegate
            {
                OnDidDismiss = HandleDismiss
            };

            NavigationItem.Title = string.Empty;
            NavigationController.Title = string.Empty;

            SetThemes();
        }

        public IQuestionPO QuestionPO
        {
            get => _questionPO;
            set
            {
                _questionPO = value;
                SetShortAnswers(_questionPO.Question.ShortAnswers);
                SetSolutionLabel();
                SetStandardAnswers(_questionPO.Question.Answers);
                SetImage();
                SetQuestionText();
                SetQuestionSubtext();
                SetLink();
            }
        }

        private void SetSolutionLabel()
        {
            var label = new UILabel();

            label.ApplyTextTheme(AppTheme.Subtitle2Label3.LightThemeOnly());
            label.Text = _questionPO.Question.SolutionTextPlaceholder;
            label.TextAlignment = UITextAlignment.Center;
            
            AnswersStackView.AddArrangedSubview(label);
            AnswersStackView.AddArrangedSubview(CreateMargin(8));
        }

        private void SetStandardAnswers(List<Answer> questionAnswers)
        {
            foreach (var answer in questionAnswers)
            {
                var answerView = new AnswerView(answer => ViewModel!.AnswerSelectedCommand.Execute(answer));
                answerView.DataContext = answer;
                AnswersStackView.AddArrangedSubview(answerView);
            }
        }

        private void AnimateBackgroundChange(AnswerView s)
        {
            var transition = new CATransition
            {
                Duration = 0.5f,
                Type = new NSString(Fade),
                TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
            };
    
            BackgroundImage.Layer.AddAnimation(transition, null);
            BackgroundImage.Image = UIImage.FromBundle(ImageResourceNames.ImageQuizBackgroundTwo.ToStandardIosImageName());
            s.SetAlphaToBackground();
        }

        private void SetQuestionSubtext()
        {
            if (_questionPO.Question.QuestionSubtext.IsNullOrEmpty())
                return;
            
            var label = new UILabel();
            label.ApplyTextTheme(AppTheme.Subtitle1Label1.LightThemeOnly());
            label.Text = _questionPO.Question.QuestionSubtext;
            label.TextAlignment = UITextAlignment.Center;
            label.Lines = 0;
            QuestionsStackView.AddArrangedSubview(label);
        }

        private void SetQuestionText()
        {
            var label = new UILabel();
            label.ApplyTextTheme(AppTheme.Heading2.LightThemeOnly());
            label.Text = _questionPO.Question.QuestionText;
            label.TextAlignment = UITextAlignment.Center;
            label.Lines = 0;
            QuestionsStackView.AddArrangedSubview(label);
        }
        
        private void SetLink()
        {
            if (_questionPO.Question.LinkText.IsNullOrEmpty())
                return;
            
            var label = new UILabel();
            label.ApplyTextTheme(AppTheme.Title3);
            label.Text = _questionPO.Question.LinkText;
            label.TextAlignment = UITextAlignment.Center;
            label.Lines = 0;
            QuestionsStackView.AddArrangedSubview(label);
        }

        private void SetImage()
        {
            if (_questionPO.Question.QuestionImageLink.IsNullOrEmpty())
                return;
            
            var image = new MvxCachedImageView();
            image.ImagePath = _questionPO.Question.QuestionImageLink;
            var imageSize = View!.Frame.Width / 2;
            image.WidthAnchor.ConstraintEqualTo(imageSize).Active = true;
            image.HeightAnchor.ConstraintEqualTo(imageSize).Active = true;
            QuestionsStackView.AddArrangedSubview(image);
        }
        
        private void SetShortAnswers(IList<ShortAnswer> shortAnswers)
        {
            foreach (var shortAnswer in shortAnswers)
            {
                var myButton = new UIButton();

                myButton.ApplyButtonStyle(shortAnswer.HasPrimaryStyle
                    ? AppTheme.ButtonPrimary.LightThemeOnly()
                    : AppTheme.ButtonTertiaryLarge.LightThemeOnly());

                myButton.SetTitle(shortAnswer.Text, UIControlState.Normal);
                myButton.HeightAnchor.ConstraintEqualTo(56).Active = true;
                myButton.Layer.CornerRadius = 28;
                myButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    ViewModel.ShortAnswerSelectedCommand.Execute(shortAnswer);
                }));
                AnswersStackView.AddArrangedSubview(myButton);
                AnswersStackView.AddArrangedSubview(CreateMargin(8));
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController!.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController!.SetNavigationBarHidden(false, true);
        }
        
        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            CloseIconView.Layer.BorderColor = AppColors.SeparatorColor.LightOnly().CGColor;
        }
        
        private void SetThemes()
        {
            CloseIconView.Layer.BorderWidth = 0.5f;
            CloseIconView.Layer.ShadowRadius = 8;
            CloseIconView.Layer.ShadowOffset = CGSize.Empty;
            CloseIconView.Layer.ShadowOpacity = 0.1f;
            CloseIconView.BackgroundColor = AppColors.BackgroundOneColor.LightOnly();
            CloseIcon.TintColor = AppColors.LabelOneColor.LightOnly();
        }

        private void HandleDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
            ClearPresentationDelegate(presentationController);
        }
        
        private static UIView CreateMargin(int margin)
        {
            var emptyView = new UIView();
            emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
            emptyView.HeightAnchor.ConstraintEqualTo(margin).Active = true;
            return emptyView;
        }
    }
}