using BMM.Core.Extensions;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.GuardedActions.Theme.Interfaces;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Utils;
using BMM.Core.ViewModels;
using MvvmCross;

namespace BMM.Core.GuardedActions.BibleStudy;

public class ActivateRewardAction : GuardedAction, IActivateRewardAction
{
    private AchievementDetailsViewModel DataContext => this.GetDataContext();
    
    protected override async Task Execute()
    {
        if (!DataContext.AchievementPO.IsActive || !DataContext.AchievementPO.HasAnyReward)
        {
            await DataContext.CloseCommand.ExecuteAsync();
            return;
        }

        if (DataContext.AchievementPO.HasThemeReward)
        {
            var setColorThemeAction = Mvx.IoCProvider.Resolve<IChangeColorThemeAction>();
            await setColorThemeAction.ExecuteGuarded(AchievementsTools.GetColorThemeFor(DataContext.AchievementPO.AchievementType)!.Value);
            return;
        }

        var appIconSelectedAction = Mvx.IoCProvider.Resolve<IAppIconSelectedAction>();
        await appIconSelectedAction.ExecuteGuarded(AchievementsTools.GetIconTypeFor(DataContext.AchievementPO.AchievementType)!.Value);
    }
}