using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.ViewModels;

namespace BMM.Core.GuardedActions.BibleStudy.Interfaces;

public interface IActivateRewardAction : IGuardedAction, IDataContextGuardedAction<AchievementDetailsViewModel>
{
}