using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.ViewModels.Parameters.Interface;

namespace BMM.Core.ViewModels.Parameters;

public class AchievementDetailsParameter : IAchievementDetailsParameter
{
    public AchievementDetailsParameter(AchievementPO achievementPO, bool showAsModal)
    {
        AchievementPO = achievementPO;
        ShowAsModal = showAsModal;
    }
    
    public AchievementPO AchievementPO { get; }
    public bool ShowAsModal { get; }
}