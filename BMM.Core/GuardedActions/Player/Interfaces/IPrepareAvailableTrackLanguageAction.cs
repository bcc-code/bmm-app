using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Player.Interfaces
{
    public interface IPrepareAvailableTrackLanguageAction 
        : IGuardedAction,
          IDataContextGuardedAction<IChangeTrackLanguageViewModel>
    {
    }
}