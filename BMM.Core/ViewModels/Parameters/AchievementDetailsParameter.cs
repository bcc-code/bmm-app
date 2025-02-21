using System.Runtime.CompilerServices;
using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters;

public class AchievementDetailsParameter : IAchievementDetailsParameter
{
    public AchievementDetailsParameter(
        AchievementPO achievementPO)
    {
        AchievementPO = achievementPO;
    }

    public AchievementDetailsParameter(string id)
    {
        Id = id;
    }

    public string Id { get; }
    public AchievementPO AchievementPO { get; }
}