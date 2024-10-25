using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Question.Interfaces
{
    public interface IInitializeQuizQuestionViewModelAction
        : IGuardedAction,
          IDataContextGuardedAction<IQuizQuestionViewModel>
    {
    }
}