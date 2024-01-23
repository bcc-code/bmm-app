using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.Achievements.Interfaces;

public interface IPrepareAchievementsAction
    : IGuardedAction,
      IDataContextGuardedAction<AchievementsViewModel>
{
    
}