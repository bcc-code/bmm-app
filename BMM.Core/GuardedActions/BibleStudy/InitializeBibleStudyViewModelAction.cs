using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.GuardedActions.TrackInfo.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Factories.Streak;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Models.POs.Other;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Utils;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.BibleStudy;

public class InitializeBibleStudyViewModelAction : GuardedAction, IInitializeBibleStudyViewModelAction
{
    private readonly IStatisticsClient _statisticsClient;
    private readonly IListeningStreakPOFactory _listeningStreakPOFactory;
    private readonly IBuildTrackInfoSectionsAction _buildTrackInfoSectionsAction;
    private readonly IBuildExternalRelationsAction _buildExternalRelationsAction;
    private readonly IMvxNavigationService _mvxNavigationService;
    private readonly IDeepLinkHandler _deepLinkHandler;
    private readonly IUriOpener _uriOpener;
    private readonly IAppLanguageProvider _appLanguageProvider;
    private readonly IDeviceInfo _deviceInfo;
    private readonly IMediaPlayer _mediaPlayer;

    private IBibleStudyViewModel DataContext => this.GetDataContext();

    public InitializeBibleStudyViewModelAction(
        IStatisticsClient statisticsClient,
        IListeningStreakPOFactory listeningStreakPOFactory,
        IBuildTrackInfoSectionsAction buildTrackInfoSectionsAction,
        IBuildExternalRelationsAction buildExternalRelationsAction,
        IMvxNavigationService mvxNavigationService,
        IDeepLinkHandler deepLinkHandler,
        IUriOpener uriOpener,
        IAppLanguageProvider appLanguageProvider,
        IDeviceInfo deviceInfo,
        IMediaPlayer mediaPlayer)
    {
        _statisticsClient = statisticsClient;
        _listeningStreakPOFactory = listeningStreakPOFactory;
        _buildTrackInfoSectionsAction = buildTrackInfoSectionsAction;
        _buildExternalRelationsAction = buildExternalRelationsAction;
        _mvxNavigationService = mvxNavigationService;
        _deepLinkHandler = deepLinkHandler;
        _uriOpener = uriOpener;
        _appLanguageProvider = appLanguageProvider;
        _deviceInfo = deviceInfo;
        _mediaPlayer = mediaPlayer;
    }
    
    protected override async Task Execute()
    {
        var projectProgress = await _statisticsClient.GetProjectProgress(_appLanguageProvider.GetAppLanguage(), await _deviceInfo.GetCurrentTheme());

        UpdateUnlockedAchievements(projectProgress);

        var track = DataContext.NavigationParameter.Track;

        DataContext.Items.Add(new BibleStudyHeaderPO(track.Album, track.Title, track.GetPublishDate()));
        
        var externalRelations = await _buildExternalRelationsAction
            .ExecuteGuarded(track);

        var externalRelationsPOItems = externalRelations
            .OfType<ExternalRelationListItemPO>()
            .ToList();

        foreach (var externalRelationItem in externalRelationsPOItems)
        {
            DataContext.Items.Add(new BibleStudyExternalRelationPO(
                externalRelationItem.TrackRelationExternal.Name,
                new Uri(externalRelationItem.TrackRelationExternal.Url),
                _deepLinkHandler,
                _uriOpener,
                _mediaPlayer));
        }

        var streak = _listeningStreakPOFactory.Create(projectProgress.Streak);
        DataContext.Items.Add(new BibleStudyProgressPO(streak, projectProgress, _mvxNavigationService));
        DataContext.Items.AddRange(await _buildTrackInfoSectionsAction.ExecuteGuarded(track));
    }

    private void UpdateUnlockedAchievements(ProjectProgress projectProgress)
    {
        foreach (var achievement in projectProgress.Achievements.Where(a => a.HasAchieved))
            AchievementsTools.SetAchievementUnlocked(achievement.Id);
    }
}