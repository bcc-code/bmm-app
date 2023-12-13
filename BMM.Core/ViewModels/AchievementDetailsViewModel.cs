using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.Utils;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Core;

namespace BMM.Core.ViewModels;

public class AchievementDetailsViewModel : BaseViewModel<IAchievementDetailsParameter>
{
    private readonly IAcknowledgeAchievementAction _acknowledgeAchievementAction;
    private readonly IMediaPlayer _mediaPlayer;
    private readonly ITracksClient _tracksClient;
    private AchievementPO _achievementPO;
    private IMvxAsyncCommand _buttonClickedCommand;
    private bool _shouldShowConfetti;
    private bool _isCurrentlyPlaying;
    private bool _shouldShowPlayNextButton;

    public AchievementDetailsViewModel(
        IAcknowledgeAchievementAction acknowledgeAchievementAction,
        IActivateRewardAction activateRewardAction,
        IMediaPlayer mediaPlayer,
        ITracksClient tracksClient)
    {
        _acknowledgeAchievementAction = acknowledgeAchievementAction;
        _mediaPlayer = mediaPlayer;
        _tracksClient = tracksClient;
        activateRewardAction.AttachDataContext(this);
        ButtonClickedCommand = activateRewardAction.Command;
        
        PlayNextClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            int? trackId = NavigationParameter.AchievementPO.TrackId;
            
            if (trackId == null)
                return;

            if (_mediaPlayer.CurrentTrack?.Id == trackId)
            {
                _mediaPlayer.PlayPause();
            }
            else
            {
                var track = await _tracksClient.GetById(trackId.Value);
                await _mediaPlayer.Play(new[] { track }, track, nameof(AchievementDetailsViewModel));
            }

            await RefreshState();
        });
    }

    public bool ShouldShowPlayNextButton
    {
        get => _shouldShowPlayNextButton;
        set => SetProperty(ref _shouldShowPlayNextButton, value);
    }

    public override void ViewDestroy(bool viewFinishing = true)
    {
        base.ViewDestroy(viewFinishing);
        AchievementsTools.IsCurrentlyShowing = false;
    }

    public override async Task Initialize()
    {
        await base.Initialize();
        AchievementPO = NavigationParameter.AchievementPO;
        ShouldShowPlayNextButton = NavigationParameter.AchievementPO.TrackId.HasValue;
        await _acknowledgeAchievementAction.ExecuteGuarded(NavigationParameter.AchievementPO);
        ShouldShowConfetti = AchievementPO.IsActive && !AchievementPO.IsAcknowledged;
        await RefreshState();
    }

    public AchievementPO AchievementPO
    {
        get => _achievementPO;
        set => SetProperty(ref _achievementPO, value);
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
    
    public Task RefreshState()
    {
        int? trackId = NavigationParameter.AchievementPO.TrackId;
        
        if (trackId == null)
            return Task.CompletedTask;
        
        bool isCurrentlySelected = _mediaPlayer.CurrentTrack != null && _mediaPlayer.CurrentTrack.Id.Equals(trackId);
        IsCurrentlyPlaying = isCurrentlySelected && _mediaPlayer.IsPlaying;
        
        return Task.CompletedTask;
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