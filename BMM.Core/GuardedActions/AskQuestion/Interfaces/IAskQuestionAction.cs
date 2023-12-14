using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.AskQuestion.Interfaces;

public interface IAskQuestionAction : IGuardedAction, IDataContextGuardedAction<AskQuestionViewModel>
{
}