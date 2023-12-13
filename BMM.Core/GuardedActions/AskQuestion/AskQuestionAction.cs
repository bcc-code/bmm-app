using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.AskQuestion.Interfaces;
using BMM.Core.GuardedActions.Base;
using BMM.Core.ViewModels;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.AskQuestion;

public class AskQuestionAction
    : GuardedAction,
      IAskQuestionAction
{
    private readonly IMvxNavigationService _mvxNavigationService;
    private readonly IQuestionsClient _questionsClient;

    public AskQuestionAction(
        IMvxNavigationService mvxNavigationService,
        IQuestionsClient questionsClient)
    {
        _mvxNavigationService = mvxNavigationService;
        _questionsClient = questionsClient;
    }
    
    private AskQuestionViewModel DataContext => this.GetDataContext();
    
    protected override async Task Execute()
    {
        bool isSuccess = await _questionsClient.PostQuestion(new PostQuestion()
        {
            Question = DataContext.Question 
        });
        
        if (isSuccess)
            await _mvxNavigationService.Navigate<AskQuestionConfirmationViewModel>();
    }
    
    protected override bool CanExecute()
    {
        return base.CanExecute() && !DataContext.Question.IsNullOrEmpty();
    }
}