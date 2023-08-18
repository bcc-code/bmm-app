using BMM.Api.Implementation.Models.Enums;
using BMM.Core.Models.Enums;

namespace BMM.Core.Utils;

public static class AchievementsTools
{
    public static AppIconType? GetIconTypeFor(string achievementType)
    {
        return achievementType switch
        {
            "FirstTest" => AppIconType.IconDarkGreen,
            "ThemeOne" => AppIconType.IconOrange,
            _ => null
        };
    }
    
    public static bool IsCurrentlyShowing { get; set; } 
}