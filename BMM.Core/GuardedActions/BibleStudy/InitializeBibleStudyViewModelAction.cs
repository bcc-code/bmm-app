using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.GuardedActions.TrackInfo.Interfaces;
using BMM.Core.Implementations.Factories.Streak;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Interfaces;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.BibleStudy;

public class InitializeBibleStudyViewModelAction : GuardedAction, IInitializeBibleStudyViewModelAction
{
    private readonly IStatisticsClient _statisticsClient;
    private readonly IListeningStreakPOFactory _listeningStreakPOFactory;
    private readonly IBuildTrackInfoSectionsAction _buildTrackInfoSectionsAction;
    private readonly IMvxNavigationService _mvxNavigationService;
    
    private IBibleStudyViewModel DataContext => this.GetDataContext();

    public InitializeBibleStudyViewModelAction(
        IStatisticsClient statisticsClient,
        IListeningStreakPOFactory listeningStreakPOFactory,
        IBuildTrackInfoSectionsAction buildTrackInfoSectionsAction,
        IMvxNavigationService mvxNavigationService)
    {
        _statisticsClient = statisticsClient;
        _listeningStreakPOFactory = listeningStreakPOFactory;
        _buildTrackInfoSectionsAction = buildTrackInfoSectionsAction;
        _mvxNavigationService = mvxNavigationService;
    }
    
    protected override async Task Execute()
    {
        var projectProgress = await _statisticsClient.GetProjectProgress();
        var track = DataContext.NavigationParameter.Track;
        
        DataContext.Items.Add(new BibleStudyHeaderPO(track.Album, track.Title, track.GetPublishDate()));
        var streak = _listeningStreakPOFactory.Create(projectProgress.Streak);
        DataContext.Items.Add(new BibleStudyProgressPO(streak, projectProgress, _mvxNavigationService));
        DataContext.Items.AddRange(await _buildTrackInfoSectionsAction.ExecuteGuarded(track));
    }
}