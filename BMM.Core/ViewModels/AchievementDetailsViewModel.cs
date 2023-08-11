using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Translation;
using BMM.Core.Utils;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels;

public class AchievementDetailsViewModel : BaseViewModel<IAchievementDetailsParameter>
{
    private readonly IAcknowledgeAchievementAction _acknowledgeAchievementAction;
    private AchievementPO _achievementPO;
    private IMvxAsyncCommand _buttonClickedCommand;
    private bool _shouldShowConfetti;

    public AchievementDetailsViewModel(
        IAcknowledgeAchievementAction acknowledgeAchievementAction)
    {
        _acknowledgeAchievementAction = acknowledgeAchievementAction;
        
        ButtonClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            if (!AchievementPO.IsActive || !AchievementPO.HasIconReward)
            {
                await CloseCommand.ExecuteAsync();
                return;
            }

            var appIconSelectedAction = Mvx.IoCProvider.Resolve<IAppIconSelectedAction>();
            await appIconSelectedAction.ExecuteGuarded(AchievementsTools.GetIconTypeFor(AchievementPO.AchievementType)!.Value);
        });
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
        await _acknowledgeAchievementAction.ExecuteGuarded(NavigationParameter.AchievementPO);
        ShouldShowConfetti = AchievementPO.IsActive && !AchievementPO.IsAcknowledged;
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

    public string ButtonTitle => GetButtonTitle();
    
    private string GetButtonTitle()
    {
        if (!AchievementPO.IsActive)
            return TextSource[nameof(Translations.AchievementDetailsViewModel_GotIt)];
        
        if (AchievementPO.HasIconReward)
            return TextSource[nameof(Translations.AchievementDetailsViewModel_ActivatePremiumIcon)];
        
        return TextSource[nameof(Translations.AchievementDetailsViewModel_Close)];
    }
}