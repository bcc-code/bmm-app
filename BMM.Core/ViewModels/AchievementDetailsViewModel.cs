using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Device;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.NewMediaPlayer;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.Utils;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels;

public class AchievementDetailsViewModel : DocumentsViewModel, IBaseViewModel<IAchievementDetailsParameter>
{
    private readonly IAcknowledgeAchievementAction _acknowledgeAchievementAction;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly IStatisticsClient _statisticsClient;
    private readonly IDeviceInfo _deviceInfo;
    private AchievementPO _achievementPO;
    private IMvxAsyncCommand _buttonClickedCommand;
    private bool _shouldShowConfetti;
    private bool _isCurrentlyPlaying;
    private bool _shouldShowPlayNextButton;
    private readonly MvxSubscriptionToken _playbackStatusChangedMessageToken;

    public AchievementDetailsViewModel(
        IAcknowledgeAchievementAction acknowledgeAchievementAction,
        IActivateRewardAction activateRewardAction,
        IMediaPlayer mediaPlayer,
        ITracksClient tracksClient,
        IStatisticsClient statisticsClient,
        IDeviceInfo deviceInfo)
    {
        _acknowledgeAchievementAction = acknowledgeAchievementAction;
        _mediaPlayer = mediaPlayer;
        _statisticsClient = statisticsClient;
        _deviceInfo = deviceInfo;
        activateRewardAction.AttachDataContext(this);
        ButtonClickedCommand = activateRewardAction.Command;
        _playbackStatusChangedMessageToken = Messenger.Subscribe<PlaybackStatusChangedMessage>(PlaybackStateChanged);
        
        PlayNextClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            int? trackId = AchievementPO.TrackId;
            
            if (trackId == null)
                return;

            if (_mediaPlayer.CurrentTrack?.Id == trackId)
            {
                _mediaPlayer.PlayPause();
            }
            else
            {
                var track = await tracksClient.GetById(trackId.Value);
                await _mediaPlayer.Play(new[] { track }, track, nameof(AchievementDetailsViewModel));
            }
        });
    }
    
    public IAchievementDetailsParameter NavigationParameter { get; private set; }

    public bool ShouldShowPlayNextButton
    {
        get => _shouldShowPlayNextButton;
        set => SetProperty(ref _shouldShowPlayNextButton, value);
    }

    public void Prepare(IAchievementDetailsParameter parameter)
    {
        NavigationParameter = parameter;
    }

    public override void ViewDestroy(bool viewFinishing = true)
    {
        base.ViewDestroy(viewFinishing);
        AchievementsTools.IsCurrentlyShowing = false;
    }
    
    private void PlaybackStateChanged(PlaybackStatusChangedMessage playbackStatusChangedMessage)
    {
        if (playbackStatusChangedMessage.PlaybackState.PlayStatus.IsOneOf(PlayStatus.Playing, PlayStatus.Paused, PlayStatus.Stopped))
            RefreshTrack();
    }

    public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
    {
        await LoadAchievement();
        ShouldShowPlayNextButton = AchievementPO.TrackId.HasValue;
        await _acknowledgeAchievementAction.ExecuteGuarded(AchievementPO);
        ShouldShowConfetti = AchievementPO.IsActive && !AchievementPO.IsAcknowledged;
        RefreshTrack();
        return Enumerable.Empty<IDocumentPO>();
    }

    private async Task LoadAchievement()
    {
        if (NavigationParameter.AchievementPO != null)
        {
            AchievementPO = NavigationParameter.AchievementPO;
            return;
        }
        
        var achievement = await _statisticsClient.GetAchievement(NavigationParameter.Id, await _deviceInfo.GetCurrentTheme());
        AchievementPO = new AchievementPO(achievement, NavigationService);
    }

    public AchievementPO AchievementPO
    {
        get => _achievementPO;
        set
        {
            SetProperty(ref _achievementPO, value);
            RaisePropertyChanged(nameof(ButtonTitle));
        }
    }

    public IMvxAsyncCommand ButtonClickedCommand
    {
        get => _buttonClickedCommand;
        set => SetProperty(ref _buttonClickedCommand, value);
    }

    public bool ShouldShowConfetti
    {
        get => _shouldShowConfetti;
        set => SetProperty(ref _shouldShowConfetti, value);
    }
    
    public bool IsCurrentlyPlaying
    {
        get => _isCurrentlyPlaying;
        set => SetProperty(ref _isCurrentlyPlaying, value);
    }
    
    public IMvxAsyncCommand PlayNextClickedCommand { get; }

    public string ButtonTitle => GetButtonTitle();

    protected override void RefreshTrackWithId(int? currentTrackId)
    {
        base.RefreshTrackWithId(currentTrackId);
        RefreshTrack();
    }

    private void RefreshTrack()
    {
        int? trackId = AchievementPO.TrackId;

        if (trackId == null)
            return;
        
        bool isCurrentlySelected = _mediaPlayer.CurrentTrack != null && _mediaPlayer.CurrentTrack.Id.Equals(trackId);
        IsCurrentlyPlaying = isCurrentlySelected && _mediaPlayer.IsPlaying;
    }

    private string GetButtonTitle()
    {
        if (!AchievementPO.IsActive)
            return TextSource[nameof(Translations.AchievementDetailsViewModel_GotIt)];
        
        if (AchievementPO.HasIconReward)
            return TextSource[nameof(Translations.AchievementDetailsViewModel_ActivatePremiumIcon)];
        
        if (AchievementPO.HasThemeReward)
            return TextSource[nameof(Translations.AchievementDetailsViewModel_ActivateTheme)];
        
        return TextSource[nameof(Translations.AchievementDetailsViewModel_Close)];
    }
}