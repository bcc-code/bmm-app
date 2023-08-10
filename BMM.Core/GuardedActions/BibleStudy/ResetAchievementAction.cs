using Acr.UserDialogs;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Implementations.Dialogs;

namespace BMM.Core.GuardedActions.BibleStudy;

public class ResetAchievementAction : GuardedAction, IResetAchievementAction
{
    private readonly IStatisticsClient _statisticsClient;
    private readonly IUserDialogs _userDialogs;

    public ResetAchievementAction(
        IStatisticsClient statisticsClient,
        IUserDialogs userDialogs)
    {
        _statisticsClient = statisticsClient;
        _userDialogs = userDialogs;
    }
    
    protected override async Task Execute()
    {
        await _statisticsClient.DeleteAchievements();
        _userDialogs.Toast("Achievements have been reset");
    }
}