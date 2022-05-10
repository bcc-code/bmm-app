using BMM.Core.GuardedActions.Base.Interfaces;
using Intents;

namespace BMM.UI.iOS.Actions.Interfaces
{
    public interface IHandleSiriMediaPlayRequestAction : IGuardedActionWithParameterAndResult<INPlayMediaIntent, INPlayMediaIntentResponseCode>
    {
    }
}