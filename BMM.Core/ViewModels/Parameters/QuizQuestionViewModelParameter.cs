using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters;

public class QuizQuestionViewModelParameter : IQuizQuestionViewModelParameter
{
    public QuizQuestionViewModelParameter(int questionId)
    {
        QuestionId = questionId;
    }
    
    public int QuestionId { get; }
}