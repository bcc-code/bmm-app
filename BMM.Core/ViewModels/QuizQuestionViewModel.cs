using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Question.Interfaces;
using BMM.Core.Models.POs.Question.Interfaces;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels;

public class QuizQuestionViewModel
    : BaseViewModel<IQuizQuestionViewModelParameter>,
      IQuizQuestionViewModel
{
    private readonly IInitializeQuizQuestionViewModelAction _initializeQuizQuestionViewModelAction;
    private readonly IStandardAnswerSelectedAction _standardAnswerSelectedAction;
    private readonly IShortAnswerSelectedAction _shortAnswerSelectedAction;
    private IQuestionPO _questionPO;

    public QuizQuestionViewModel(
        IInitializeQuizQuestionViewModelAction initializeQuizQuestionViewModelAction,
        IStandardAnswerSelectedAction standardAnswerSelectedAction,
        IShortAnswerSelectedAction shortAnswerSelectedAction)
    {
        _initializeQuizQuestionViewModelAction = initializeQuizQuestionViewModelAction;
        _standardAnswerSelectedAction = standardAnswerSelectedAction;
        _shortAnswerSelectedAction = shortAnswerSelectedAction;
        _standardAnswerSelectedAction.AttachDataContext(this);
        _shortAnswerSelectedAction.AttachDataContext(this);
        _initializeQuizQuestionViewModelAction.AttachDataContext(this);
    }
    
    public int Tries { get; set; }
    public string InitialAnswerId { get; set; }
    public IMvxAsyncCommand<Answer> AnswerSelectedCommand => _standardAnswerSelectedAction.Command;
    public IMvxAsyncCommand<ShortAnswer> ShortAnswerSelectedCommand => _shortAnswerSelectedAction.Command;

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