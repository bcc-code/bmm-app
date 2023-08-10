using BMM.Api.Implementation.Models.Enums;
using BMM.Core.GuardedActions.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;

namespace BMM.Core.GuardedActions.BibleStudy.Interfaces;

public interface IAcknowledgeAchievementAction : IGuardedActionWithParameter<AchievementPO>
{
}