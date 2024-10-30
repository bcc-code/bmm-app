using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Question.Interfaces;

public interface IStandardAnswerSelectedAction
    : IGuardedActionWithParameter<Answer>,
      IDataContextGuardedAction<IQuizQuestionViewModel>
{
}