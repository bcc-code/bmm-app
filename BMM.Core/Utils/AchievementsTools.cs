using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Models.Enums;

namespace BMM.Core.Utils;

public static class AchievementsTools
{
    public static AppIconType? GetIconTypeFor(AchievementType achievementType)
    {
        return achievementType switch
        {
            AchievementType.FirstTest => AppIconType.DarkGreen,
            AchievementType.ThemeOne => AppIconType.Violet,
            _ => null
        };
    }
    
    public static bool IsCurrentlyShowing { get; set; } 
}