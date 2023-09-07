using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.Enums;
using BMM.Core.Models.Themes;
using BMM.Core.Translation;
using Microsoft.Maui.Devices;

namespace BMM.Core.Utils;

public static class AchievementsTools
{
    private const string ThemeOne = nameof(ThemeOne);
    private const string Points50 = nameof(Points50);
    private const string ThemeTwo = nameof(ThemeTwo);
    private const string Points100 = nameof(Points100);
    private const string Points180 = nameof(Points180);
    
    public static AppIconType? GetIconTypeFor(string achievementType)
    {
        return achievementType switch
        {
            ThemeOne => AppIconType.IconDarkGreen,
            Points50 => AppIconType.IconOrange,
            ThemeTwo => AppIconType.IconViolet,
            Points100 => AppIconType.IconRed,
            Points180 => AppIconType.IconGolden,
            _ => null
        };
    }
    
    public static ColorTheme? GetColorThemeFor(string achievementType)
    {
        return achievementType switch
        {
            ThemeOne => ColorTheme.DarkGreen,
            Points50 => ColorTheme.Orange,
            ThemeTwo => ColorTheme.Violet,
            Points100 => ColorTheme.Red,
            Points180 => ColorTheme.Golden,
            _ => null
        };
    }
    
    public static void SetAchievementUnlocked(string achievementType)
    {
        switch (achievementType)
        {
            case ThemeOne:
                AppSettings.DarkGreenRewardUnlocked = true;
                break;
            case Points50:
                AppSettings.OrangeRewardUnlocked = true;
                break;
            case ThemeTwo:
                AppSettings.VioletRewardUnlocked = true;
                break;
            case Points100:
                AppSettings.RedRewardUnlocked = true;
                break;
            case Points180:
                AppSettings.GoldenRewardUnlocked = true;
                break;
        }
    }
    
    public static string GetRewardDescriptionFor(string achievementType)
    {
        var languageBinder = BMMLanguageBinderLocator.TextSource;

        if (achievementType == Points180)
        {
            return DeviceInfo.Current.Platform == DevicePlatform.Android
                ? languageBinder.GetText(Translations.AchievementDetailsViewModel_RewardAndroidGolden)
                : languageBinder.GetText(Translations.AchievementDetailsViewModel_RewardIosGolden);
        }
        
        string reward = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? languageBinder.GetText(Translations.AchievementDetailsViewModel_RewardAndroid)
            : languageBinder.GetText(Translations.AchievementDetailsViewModel_RewardIos);

        switch (achievementType)
        {
            case ThemeOne:
            case Points50:
            case ThemeTwo:
            case Points100:
            case Points180:
                return reward;
            default:
                return null;
        }
    }

    public static bool AnyAchievementUnlocked()
    {
        return AppSettings.DarkGreenRewardUnlocked
               || AppSettings.OrangeRewardUnlocked
               || AppSettings.VioletRewardUnlocked
               || AppSettings.RedRewardUnlocked
               || AppSettings.GoldenRewardUnlocked;
    }
    
    public static bool IsCurrentlyShowing { get; set; }
}