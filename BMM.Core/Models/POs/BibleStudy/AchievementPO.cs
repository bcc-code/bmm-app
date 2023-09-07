using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.BibleStudy.Interfaces;
using BMM.Core.Utils;
using BMM.Core.ViewModels;
using BMM.Core.ViewModels.Parameters;
using BMM.Core.ViewModels.Parameters.Interface;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Maui.Devices;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace BMM.Core.Models.POs.BibleStudy;

public class AchievementPO : BasePO, IAchievementPO
{
    private readonly Achievement _achievement;

    public AchievementPO(
        Achievement achievement,
        IMvxNavigationService navigationService)
    {
        _achievement = achievement;
        AchievementClickedCommand = new ExceptionHandlingCommand(async () =>
        {
            await navigationService.Navigate<AchievementDetailsViewModel, IAchievementDetailsParameter>(new AchievementDetailsParameter(
                this,
                false));
        });
    }

    public string ImagePath => _achievement.Url;
    public bool IsActive => _achievement.HasAchieved;
    public string AchievementType => _achievement.Id;
    public bool IsAcknowledged => _achievement.HasAcknowledged;
    public string Title => _achievement.Title;
    public string Description => _achievement.Description;
    public bool HasIconReward => DeviceInfo.Current.Platform == DevicePlatform.iOS && AchievementsTools.GetIconTypeFor(_achievement.Id) != null;
    public bool HasThemeReward => DeviceInfo.Current.Platform == DevicePlatform.Android && AchievementsTools.GetColorThemeFor(_achievement.Id) != null;
    public bool HasAnyReward => HasIconReward || HasThemeReward;
    public string RewardDescription => AchievementsTools.GetRewardDescriptionFor(_achievement.Id);
    public IMvxAsyncCommand AchievementClickedCommand { get; }
    public bool ShouldShowRewardDescription => !RewardDescription.IsNullOrEmpty() && !IsActive;
    public bool ShouldShowSecondRewardDescription => !RewardDescription.IsNullOrEmpty() && IsActive;
}