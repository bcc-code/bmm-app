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
    AchievementType AchievementType { get; }
    IMvxAsyncCommand AchievementClickedCommand { get; }
}