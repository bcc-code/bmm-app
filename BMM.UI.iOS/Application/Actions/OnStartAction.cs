using System.Threading.Tasks;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.Base;

namespace BMM.UI.iOS.Actions
{
    /// <summary>
    /// Action that runs directly after app opening.
    /// iOS Placeholder for future use.
    /// </summary>
    public class OnStartAction : GuardedAction, IOnStartAction
    {
        protected override Task Execute()
        {
            return Task.CompletedTask;
        }
    }
}