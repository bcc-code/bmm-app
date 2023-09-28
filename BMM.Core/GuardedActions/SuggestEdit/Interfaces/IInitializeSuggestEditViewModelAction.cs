using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.SuggestEdit.Interfaces;

public interface IInitializeSuggestEditViewModelAction
    : IGuardedAction,
      IDataContextGuardedAction<SuggestEditViewModel>
{
}