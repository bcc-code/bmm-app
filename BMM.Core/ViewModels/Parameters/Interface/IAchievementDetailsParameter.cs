using BMM.Core.Models.POs.BibleStudy;

namespace BMM.Core.ViewModels.Parameters.Interface;

public interface IAchievementDetailsParameter
{
    string Id { get; }
    AchievementPO AchievementPO { get; }
}