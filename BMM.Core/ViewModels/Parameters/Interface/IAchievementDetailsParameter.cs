using BMM.Core.Models.POs.BibleStudy;

namespace BMM.Core.ViewModels.Parameters.Interface;

public interface IAchievementDetailsParameter
{
    AchievementPO AchievementPO { get; }
    bool ShowAsModal { get; }
}