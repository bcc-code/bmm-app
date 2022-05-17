using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.SiriShortcuts.Interfaces
{
    public interface IInitializeSiriShortcutsSettingsAction
        : IGuardedAction,
          IDataContextGuardedAction<ISiriShortcutsViewModel>
    {
    }
}