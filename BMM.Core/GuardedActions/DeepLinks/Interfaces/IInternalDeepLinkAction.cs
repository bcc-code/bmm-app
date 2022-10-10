using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.GuardedActions.DeepLinks.Parameters;

namespace BMM.Core.GuardedActions.DeepLinks.Interfaces
{
    public interface IInternalDeepLinkAction : IGuardedActionWithParameter<InternalDeepLinkActionParameter>
    {
    }
}