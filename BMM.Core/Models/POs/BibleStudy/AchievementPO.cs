using BMM.Core.Helpers;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.BibleStudy;

public class AchievementPO : BasePO, IAchievementPO
{
    public AchievementPO(
        string imageName,
        bool isActive,
        IMvxNavigationService navigationService)
    {
        ImageName = imageName;
        IsActive = isActive;
        AchievementClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            await navigationService.Navigate<AchievementDetailsViewModel>();
        });
    }
    
    public string ImageName { get; }
    public bool IsActive { get; }
    public IMvxAsyncCommand AchievementClickedCommand { get; }
}