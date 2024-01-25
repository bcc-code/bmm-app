using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Achievements.Interfaces;
using BMM.Core.GuardedActions.Base;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Languages;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Other;
using BMM.Core.ViewModels;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.Achievements;

public class PrepareAchievementsAction
    : GuardedAction,
      IPrepareAchievementsAction
{
    private readonly IStatisticsClient _statisticsClient;
    private readonly IAppLanguageProvider _appLanguageProvider;
    private readonly IDeviceInfo _deviceInfo;
    private readonly IMvxNavigationService _mvxNavigationService;

    public PrepareAchievementsAction(
        IStatisticsClient statisticsClient,
        IAppLanguageProvider appLanguageProvider,
        IDeviceInfo deviceInfo,
        IMvxNavigationService mvxNavigationService)
    {
        _statisticsClient = statisticsClient;
        _appLanguageProvider = appLanguageProvider;
        _deviceInfo = deviceInfo;
        _mvxNavigationService = mvxNavigationService;
    }

    private AchievementsViewModel DataContext => this.GetDataContext();
    
    protected override async Task Execute()
    {
        var achievementHolder = await _statisticsClient.GetAchievements(
            _appLanguageProvider.GetAppLanguage(),
            await _deviceInfo.GetCurrentTheme());

        var elementsList = new List<BasePO>();

        foreach (var achievementElement in achievementHolder.Items)
        {
            switch (achievementElement)
            {
                case ChapterHeader header:
                    elementsList.Add(new ChapterHeaderPO(header));
                    break;
                case AchievementsCollection achievementsCollection:
                {
                    foreach (var achievement in achievementsCollection.List)
                    {
                        elementsList.Add(new AchievementPO(achievement, _mvxNavigationService));
                        elementsList.Add(new AchievementPO(achievement, _mvxNavigationService));
                        elementsList.Add(new AchievementPO(achievement, _mvxNavigationService));
                    }

                    break;
                }
            }
        }
        
        DataContext.Achievements.AddRange(elementsList);
    }
}