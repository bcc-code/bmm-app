using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations.Languages;
using BMM.Core.Messages;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Utils;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.GuardedActions.BibleStudy;

public class CheckAndShowAchievementUnlockedScreenAction : GuardedAction, ICheckAndShowAchievementUnlockedScreenAction
{
    private readonly IStatisticsClient _statisticsClient;
    private readonly IMvxNavigationService _mvxNavigationService;
    private readonly IMvxMessenger _mvxMessenger;
    private readonly IAppLanguageProvider _appLanguageProvider;

    public CheckAndShowAchievementUnlockedScreenAction(
        IStatisticsClient statisticsClient,
        IMvxNavigationService mvxNavigationService,
        IMvxMessenger mvxMessenger,
        IAppLanguageProvider appLanguageProvider)
    {
        _statisticsClient = statisticsClient;
        _mvxNavigationService = mvxNavigationService;
        _mvxMessenger = mvxMessenger;
        _appLanguageProvider = appLanguageProvider;
    }
    
    protected override async Task Execute()
    {
        var projectProgress = await _statisticsClient.GetProjectProgress(_appLanguageProvider.GetAppLanguage());
        
        if (!projectProgress.Achievements.Any())
            return;

        AssignRewardPermissions(projectProgress);
        
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

    private void AssignRewardPermissions(ProjectProgress projectProgress)
    {
        foreach (var achievement in projectProgress.Achievements.Where(a => !a.HasAchieved))
            AchievementsTools.SetAchievementUnlocked(achievement.Id);
    }
}