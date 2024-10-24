using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Question.Interfaces;
using BMM.Core.Models.POs.Question;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Question;

public class InitializeQuizQuestionViewModelAction
    : GuardedAction,
      IInitializeQuizQuestionViewModelAction
{
    private readonly IQuestionsClient _questionsClient;
    private IQuizQuestionViewModel DataContext => this.GetDataContext();

    public InitializeQuizQuestionViewModelAction(IQuestionsClient questionsClient)
    {
        _questionsClient = questionsClient;
    }
    
    protected override async Task Execute()
    {
        var question = await _questionsClient.GetQuestion(DataContext.NavigationParameter.QuestionId);
        DataContext.QuestionPO = new QuestionPO(question);
    }
}