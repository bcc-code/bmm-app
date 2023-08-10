using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations;
using BMM.Core.Messages;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Utils;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Base;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.GuardedActions.BibleStudy;

public class CheckAndShowAchievementUnlockedScreenAction : GuardedAction, IShowAchievementUnlockedScreenAction
{
    private readonly IStatisticsClient _statisticsClient;
    private readonly IMvxNavigationService _mvxNavigationService;
    private readonly IMvxMessenger _mvxMessenger;

    public CheckAndShowAchievementUnlockedScreenAction(
        IStatisticsClient statisticsClient,
        IMvxNavigationService mvxNavigationService,
        IMvxMessenger mvxMessenger)
    {
        _statisticsClient = statisticsClient;
        _mvxNavigationService = mvxNavigationService;
        _mvxMessenger = mvxMessenger;
    }
    
    protected override async Task Execute()
    {
        var projectProgress = await _statisticsClient.GetProjectProgress();
        
        if (!projectProgress.Achievements.Any())
            return;

        var achievementToShow = projectProgress
            .Achievements
            .FirstOrDefault(a => a.HasAchieved && !a.HasAcknowledged);

        if (achievementToShow == null || AchievementsTools.IsCurrentlyShowing)
            return;

        AchievementsTools.IsCurrentlyShowing = true;

        await _mvxNavigationService.ChangePresentation(new CloseFragmentsOverPlayerHint());
        _mvxMessenger.Publish(new TogglePlayerMessage(this, false));
        await Task.Delay(ViewConstants.DefaultAnimationDurationInMilliseconds);
        
        var achievementPO = new AchievementPO(achievementToShow, _mvxNavigationService);

        await _mvxNavigationService.Navigate<AchievementDetailsViewModel, IAchievementDetailsParameter>(
            new AchievementDetailsParameter(achievementPO, true));
    }
}