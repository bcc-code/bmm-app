using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.ContinueListening.Interfaces;

public interface IHandleAutoplayAction : IGuardedActionWithParameter<ContinueListeningTile>
{
}