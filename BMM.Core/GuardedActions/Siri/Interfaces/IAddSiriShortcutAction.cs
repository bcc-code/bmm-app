using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Siri.Interfaces
{
    public interface IAddSiriShortcutAction 
        : IGuardedActionWithParameter<StandardSelectablePO>,
          IDataContextGuardedAction<ISiriShortcutsViewModel>
    {
    }
}