using System.Reflection.Emit;
using _Microsoft.Android.Resource.Designer;
using Android.Animation;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Question.Interfaces;
using BMM.Core.ViewModels;
using BMM.UI.Droid.Application.Adapters;
using BMM.UI.Droid.Application.CustomViews;
using BMM.UI.Droid.Application.Extensions;
using BMM.UI.Droid.Application.Fragments.Base;
using FFImageLoading.Cross;
using FFImageLoading.Extensions;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Binding.BindingContext;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using ContextThemeWrapper = AndroidX.AppCompat.View.ContextThemeWrapper;

namespace BMM.UI.Droid.Application.Fragments
{
    [MvxDialogFragmentPresentation(ActivityHostViewModelType = typeof(MainActivityViewModel), Cancelable = false, AddToBackStack = true)]
    [Register("bmm.ui.droid.application.fragments.QuizQuestionFragment")]
    public class QuizQuestionFragment : BaseDialogFragment<QuizQuestionViewModel>
    {
        private IQuestionPO _questionPO;
        private ImageView _backgroundImage;
        protected override int FragmentId => Resource.Layout.fragment_quiz_question;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            _backgroundImage = view.FindViewById<ImageView>(Resource.Id.BackgroundImage);
            return view;
        }

        protected override void Bind()
        {
            var set = this.CreateBindingSet<QuizQuestionFragment, QuizQuestionViewModel>();
        
            set.Bind(this)
                .For(v => v.QuestionPO)
                .To(vm => vm.QuestionPO);
            
            set.Apply();
        }

        public IQuestionPO QuestionPO
        {
            get => _questionPO;
            set
            {
                _questionPO = value;
                SetSolutionLabel();
                SetShortAnswers(_questionPO.Question.ShortAnswers);
                SetStandardAnswers(_questionPO.Question.Answers);
                SetImage();
                SetQuestionText();
                SetQuestionSubtext();
                SetLink();
            }
        }

        private void SetLink()
        {
            if (_questionPO.Question.LinkText.IsNullOrEmpty())
                return;
            
            var linearLayout = View!.FindViewById<LinearLayout>(Resource.Id.QuestionsLinearLayout);
                
            var myButton = new Button(new ContextThemeWrapper(
                    Context,
                    ResourceConstant.Style.Button_Tertiary_Small),
                null,
                default);
            myButton.Text = _questionPO.Question.LinkText;
            
            var lp = new LinearLayout.LayoutParams(
                ViewGroup.MarginLayoutParams.WrapContent,
                ViewGroup.MarginLayoutParams.WrapContent);
            
            lp.Gravity = GravityFlags.CenterHorizontal;
            myButton.LayoutParameters = lp;

            linearLayout!.AddView(myButton);
        }

        private void SetQuestionSubtext()
        {
            if (_questionPO.Question.QuestionSubtext.IsNullOrEmpty())
                return;

            var linearLayout = View!.FindViewById<LinearLayout>(Resource.Id.QuestionsLinearLayout);

            var label = new TextView(new ContextThemeWrapper(
                    Context,
                    ResourceConstant.Style.Subtitle1_Label1),
                null,
                default);
            
            label.Text = _questionPO.Question.QuestionSubtext;
            label.Gravity = GravityFlags.CenterHorizontal;
            
            linearLayout!.AddView(label);
            linearLayout!.AddView(CreateMargin(16));
        }

        private void SetQuestionText()
        {
            if (_questionPO.Question.QuestionText.IsNullOrEmpty())
                return;

            var linearLayout = View!.FindViewById<LinearLayout>(Resource.Id.QuestionsLinearLayout);

            var label = new TextView(new ContextThemeWrapper(
                    Context,
                    ResourceConstant.Style.Heading2),
                null,
                default);
            
            label.Text = _questionPO.Question.QuestionText;
            label.Gravity = GravityFlags.CenterHorizontal;
            
            linearLayout!.AddView(label);
            linearLayout!.AddView(CreateMargin(16));
        }

        private void SetImage()
        {
            if (_questionPO.Question.QuestionImageLink.IsNullOrEmpty())
                return;
            
            var linearLayout = View!.FindViewById<LinearLayout>(Resource.Id.QuestionsLinearLayout);
            
            var image = new MvxCachedImageView(Context!);
            image.ImagePath = _questionPO.Question.QuestionImageLink;
            
            int imageSize = View!.MeasuredWidth / 2;
            var lp = new LinearLayout.LayoutParams(imageSize, imageSize);
            lp.Gravity = GravityFlags.CenterHorizontal;
            image.LayoutParameters = lp;
            
            linearLayout!.AddView(image);
            linearLayout!.AddView(CreateMargin(16));
        }

        private void SetStandardAnswers(List<Answer> questionAnswers)
        {
            if (!questionAnswers.Any())
                return;
            
            var linearLayout = View!.FindViewById<LinearLayout>(Resource.Id.AnswersLinearLayout);

            foreach (var answer in questionAnswers)
            {
                var answerView = new AnswerView(Context, ClickedAction);
                answerView.BindingContext.DataContext = answer;
                linearLayout!.AddView(answerView);
                linearLayout.AddView(CreateMargin(8));
            }
            
            linearLayout!.AddView(CreateMargin(16));
        }

        private void SetSolutionLabel()
        {
            if (_questionPO.Question.SolutionTextPlaceholder.IsNullOrEmpty())
                return;
            
            var linearLayout = View!.FindViewById<LinearLayout>(Resource.Id.AnswersLinearLayout);
            
            var label = new TextView(new ContextThemeWrapper(
                    Context,
                    ResourceConstant.Style.Subtitle2_Label3),
                null,
                default);

            label.Gravity = GravityFlags.CenterHorizontal;
            label.Text = _questionPO.Question.SolutionTextPlaceholder;
            
            linearLayout!.AddView(label);
            linearLayout.AddView(CreateMargin(16));
        }

        private void SetShortAnswers(List<ShortAnswer> shortAnswers)
        {
            var linearLayout = View!.FindViewById<LinearLayout>(Resource.Id.AnswersLinearLayout);
            
            foreach (var shortAnswer in shortAnswers)
            {
                int theme = shortAnswer.HasPrimaryStyle
                    ? ResourceConstant.Style.Button_Primary_Large
                    : ResourceConstant.Style.Button_Tertiary_Large;
                
                var myButton = new Button(new ContextThemeWrapper(
                    Context,
                    theme),
                    null,
                    default);
                myButton.Text = shortAnswer.Text;

                linearLayout!.AddView(myButton);
                linearLayout.AddView(CreateMargin(16));
            }
        }

        private Space CreateMargin(int size)
        {
            var emptyView = new Space(Context);
            var lp = new LinearLayout.LayoutParams(size.DpToPixels(), size.DpToPixels());
            emptyView.LayoutParameters = lp;
            return emptyView;
        }
        
        private void ClickedAction()
        {
            var currentDrawable = _backgroundImage.Drawable;
            var newDrawable = Resources.GetDrawable(Resource.Drawable.image_quiz_background_two, null);

            // Create a TransitionDrawable with the current and new image
            Drawable[] layers = { currentDrawable, newDrawable };
            TransitionDrawable transitionDrawable = new TransitionDrawable(layers);

            // Set the TransitionDrawable as the image
            _backgroundImage.SetImageDrawable(transitionDrawable);

            // Start the transition (fade duration 500ms)
            transitionDrawable.StartTransition(300);
        }
    }
}