using BMM.Core.GuardedActions.Base.Interfaces;

namespace BMM.Core.GuardedActions.Theme.Interfaces
{
    public interface IChangeThemeSettingsAction : IGuardedActionWithParameter<Models.Themes.Theme>
    {
    }
}