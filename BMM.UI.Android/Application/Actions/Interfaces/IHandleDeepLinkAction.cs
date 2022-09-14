using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.UI.Droid.Application.Actions.Interfaces
{
    public interface IHandleDeepLinkAction : IGuardedActionWithParameter<string>
    {
    }
}