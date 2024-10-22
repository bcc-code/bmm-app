using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Question.Interfaces;
using BMM.Core.Models.POs.Question.Interfaces;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels;

public class QuizQuestionViewModel
    : BaseViewModel<IQuizQuestionViewModelParameter>,
      IQuizQuestionViewModel
{
    private readonly IInitializeQuizQuestionViewModelAction _initializeQuizQuestionViewModelAction;
    private IQuestionPO _questionPO;

    public QuizQuestionViewModel(IInitializeQuizQuestionViewModelAction initializeQuizQuestionViewModelAction)
    {
        _initializeQuizQuestionViewModelAction = initializeQuizQuestionViewModelAction;
        _initializeQuizQuestionViewModelAction.AttachDataContext(this);
    }
    
    public override async Task Initialize()
    {
        await base.Initialize();
        await _initializeQuizQuestionViewModelAction.ExecuteGuarded();
    }

    public IQuestionPO QuestionPO
    {
        get => _questionPO;
        set => SetProperty(ref _questionPO, value);
    }
}