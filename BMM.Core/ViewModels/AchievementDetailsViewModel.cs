using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.UI;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.Models.Enums;
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
    private readonly ITracksClient _tracksClient;
    private readonly IStatisticsClient _statisticsClient;
    private readonly IDeviceInfo _deviceInfo;
    private AchievementPO _achievementPO;
    private IMvxAsyncCommand _buttonClickedCommand;
    private bool _shouldShowConfetti;
    private bool _isCurrentlyPlaying;
    private bool _shouldShowActionButton;
    private readonly MvxSubscriptionToken _playbackStatusChangedMessageToken;

    public AchievementDetailsViewModel(
        IAcknowledgeAchievementAction acknowledgeAchievementAction,
        IActivateRewardAction activateRewardAction,
        IMediaPlayer mediaPlayer,
        ITracksClient tracksClient,
        IStatisticsClient statisticsClient,
        IDeviceInfo deviceInfo,
        IUriOpener uriOpener)
    {
        _acknowledgeAchievementAction = acknowledgeAchievementAction;
        _mediaPlayer = mediaPlayer;
        _tracksClient = tracksClient;
        _statisticsClient = statisticsClient;
        _deviceInfo = deviceInfo;
        activateRewardAction.AttachDataContext(this);
        ButtonClickedCommand = activateRewardAction.Command;
        _playbackStatusChangedMessageToken = Messenger.Subscribe<PlaybackStatusChangedMessage>(PlaybackStateChanged);
        
        ActionButtonClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            if (AchievementPO.ActionButtonType == AchievementActionButtonType.PlayNext)
                await PlayTrack(AchievementPO.TrackId!.Value);
            else
                uriOpener.OpenUri(new Uri(AchievementPO.ActionButtonUrl));
        });
    }

    public IAchievementDetailsParameter NavigationParameter { get; private set; }

    public bool ShouldShowActionButton
    {
        get => _shouldShowActionButton;
        set => SetProperty(ref _shouldShowActionButton, value);
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
        ShouldShowActionButton = AchievementPO.HasActionButton;
        await _acknowledgeAchievementAction.ExecuteGuarded(AchievementPO);
        ShouldShowConfetti = AchievementPO.IsActive && !AchievementPO.IsAcknowledged;
        RefreshTrack();
        return Enumerable.Empty<IDocumentPO>();
    }

    private async Task PlayTrack(int trackId)
    {
        if (_mediaPlayer.CurrentTrack?.Id == trackId)
        {
            _mediaPlayer.PlayPause();
        }
        else
        {
            var track = await _tracksClient.GetById(trackId);
            await _mediaPlayer.Play([track], track, nameof(AchievementDetailsViewModel));
        }
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
    
    public IMvxAsyncCommand ActionButtonClickedCommand { get; }

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