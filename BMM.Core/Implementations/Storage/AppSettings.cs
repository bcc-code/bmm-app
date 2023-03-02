using System.Collections.Generic;
using System.Linq;
using BMM.Core.Models.Themes;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Storage
{
    public static class AppSettings
    {
        private static IPreferences Settings => Preferences.Default;

        public static Theme SelectedTheme
        {
            get => GetValueOrDefault(nameof(SelectedTheme), Theme.System);
            set => AddOrUpdateValue(value, nameof(SelectedTheme));
        }

        public static bool YearInReviewShown
        {
            get => GetValueOrDefault(nameof(YearInReviewShown), false);
            set => AddOrUpdateValue(value, nameof(YearInReviewShown));
        }
        
        public static IList<int> DismissedMessageTilesIds
        {
            get => GetValueOrDefault(nameof(DismissedMessageTilesIds), new List<int>());
            set => AddOrUpdateValue(value, nameof(DismissedMessageTilesIds));
        }

        public static void Clear() => Settings.Clear();
        
        private static void AddOrUpdateValue<TValue>(TValue value, string settingsKey)
        {
            if (value is string stringValue)
            {
                Settings.Set(settingsKey, stringValue);
                return;
            }

            string? serialized = value != null
                ? JsonConvert.SerializeObject(value)
                : null;

            Settings.Set(settingsKey, serialized);
        }

        private static TValue GetValueOrDefault<TValue>(string settingsKey, TValue defaultValue = default)
        {
            string value = Settings.Get<string>(settingsKey, null);

            switch (value)
            {
                case TValue stringValue:
                    return stringValue;
                case null:
                    return defaultValue;
                default:
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<TValue>(value);
                    }
                    catch
                    {
                        AddOrUpdateValue(defaultValue, settingsKey);
                        return defaultValue;
                    }
                }
            }
        }
    }
}