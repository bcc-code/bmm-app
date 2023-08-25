using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.Themes;

namespace BMM.Core.GuardedActions.Theme.Interfaces
{
    public interface IChangeColorThemeAction : IGuardedActionWithParameter<ColorTheme>
    {
    }
}