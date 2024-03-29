using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.BibleStudy.Interfaces;

public interface IInitializeBibleStudyViewModelAction : IGuardedAction, IDataContextGuardedAction<IBibleStudyViewModel>
{
}