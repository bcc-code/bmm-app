using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models.Enums;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Models.POs.BibleStudy;

namespace BMM.Core.GuardedActions.BibleStudy;

public class AcknowledgeAchievementAction
    : GuardedActionWithParameter<AchievementPO>,
      IAcknowledgeAchievementAction
{
    private readonly IStatisticsClient _statisticsClient;

    public AcknowledgeAchievementAction(IStatisticsClient statisticsClient)
    {
        _statisticsClient = statisticsClient;
    }
    
    protected override async Task Execute(AchievementPO parameter)
    {
        if (parameter.IsActive && !parameter.IsAcknowledged)
            await _statisticsClient.AchievementAcknowledge(parameter.AchievementType);
    }
}