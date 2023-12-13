using BMM.Core.Extensions;
using BMM.Core.GuardedActions.AskQuestion.Interfaces;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels;

public class AskQuestionViewModel : BaseViewModel
{
    private readonly IAskQuestionAction _askQuestionAction;
    private string _question;

    public AskQuestionViewModel(IAskQuestionAction askQuestionAction)
    {
        _askQuestionAction = askQuestionAction;
        _askQuestionAction.AttachDataContext(this);
    }

    public IMvxAsyncCommand SubmitCommand => _askQuestionAction.Command;
    public bool CanSubmit => SubmitCommand.CanExecute();

    public string Question
    {
        get => _question;
        set
        {
            SetProperty(ref _question, value);
            RaisePropertyChanged(() => CanSubmit);
        }
    }

    public override async Task Initialize()
    {
    }
}