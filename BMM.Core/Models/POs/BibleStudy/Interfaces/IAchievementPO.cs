using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IAchievementPO : IBasePO
{
    string ImagePath { get; }
    bool IsActive { get; }
    string Title { get; }
    string Description { get; }
    string AchievementType { get; }
    string RewardDescription { get; }
    bool ShouldShowRewardDescription { get; }
    int? TrackId { get; }
    bool HasActionButton { get; }
    string ActionButtonTitle { get; }
    IMvxAsyncCommand AchievementClickedCommand { get; }
}