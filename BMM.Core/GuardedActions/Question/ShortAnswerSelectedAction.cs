using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Question.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.ViewModels.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BMM.Core.GuardedActions.Question;

public class ShortAnswerSelectedAction
    : GuardedActionWithParameter<ShortAnswer>,
      IShortAnswerSelectedAction
{
    private const int ShortAnswerTriesCount = 1;
    private readonly IQuestionsClient _questionsClient;
    private readonly IToastDisplayer _toastDisplayer;

    public ShortAnswerSelectedAction(
        IQuestionsClient questionsClient,
        IToastDisplayer toastDisplayer)
    {
        _questionsClient = questionsClient;
        _toastDisplayer = toastDisplayer;
    }

    private IQuizQuestionViewModel QuizQuestionViewModel => this.GetDataContext();
    
    protected override async Task Execute(ShortAnswer answer)
    {
        var question = QuizQuestionViewModel
            .QuestionPO
            .Question;
        
        bool isSuccess = await _questionsClient.PostAnswer(new PostAnswer
        {
            FinalAnswerId = answer.Id,
            InitialAnswerId =  answer.Id,
            QuestionId = question.Id,
            Tries = ShortAnswerTriesCount
        });

        if (isSuccess)
        {
            await QuizQuestionViewModel.CloseCommand.ExecuteAsync();
            
            if (!question.ThankYouText.IsNullOrEmpty())
                await _toastDisplayer.Success(question.ThankYouText);
        }
    }
}