using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Achievements.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Models.POs.Base;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels;

public class AchievementsViewModel : BaseViewModel
{
    private readonly IPrepareAchievementsAction _prepareAchievementsAction;

    public AchievementsViewModel(IPrepareAchievementsAction prepareAchievementsAction)
    {
        _prepareAchievementsAction = prepareAchievementsAction;
        _prepareAchievementsAction.AttachDataContext(this);
    }

    public IBmmObservableCollection<BasePO> Achievements { get; } = new BmmObservableCollection<BasePO>();
    
    public override async Task Initialize()
    {
        await base.Initialize();
        await _prepareAchievementsAction.ExecuteGuarded();
    }
}