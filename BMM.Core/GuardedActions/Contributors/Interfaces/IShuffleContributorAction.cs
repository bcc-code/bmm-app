using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.Contributors.Interfaces;

namespace BMM.Core.GuardedActions.Contributors.Interfaces
{
    public interface IShuffleContributorAction : IGuardedActionWithParameter<IShuffleActionParameter>
    {
    }
}