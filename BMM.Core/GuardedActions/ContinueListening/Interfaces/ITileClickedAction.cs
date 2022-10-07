using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs.ContinueListening;

namespace BMM.Core.GuardedActions.ContinueListening.Interfaces
{
    public interface ITileClickedAction : IGuardedActionWithParameter<ContinueListeningTile>
    {
    }
}