using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs;

namespace BMM.Core.GuardedActions.Siri.Interfaces
{
    public interface IAddSiriShortcutAction : IGuardedActionWithParameter<StandardSelectablePO>
    {
    }
}