using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IAchievementPO : IBasePO
{
    string ImageName { get; }
    bool IsActive { get; }
    IMvxAsyncCommand AchievementClickedCommand { get; }
}