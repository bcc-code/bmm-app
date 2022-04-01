using BMM.Core.Models.Themes;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace BMM.Core.Implementations.Storage
{
    public static class AppSettings
    {
        private static ISettings Settings => CrossSettings.Current;

        public static Theme SelectedTheme
        {
            get => GetValueOrDefault(nameof(SelectedTheme), Theme.System);
            set => AddOrUpdateValue(value, nameof(SelectedTheme));
        }

        private static void AddOrUpdateValue<TValue>(TValue value, string settingsKey)
        {
            if (value is string stringValue)
            {
                Settings.AddOrUpdateValue(settingsKey, stringValue);
                return;
            }

            string serialized = value != null
                ? JsonConvert.SerializeObject(value)
                : null;

            Settings.AddOrUpdateValue(settingsKey, serialized);
        }

        private static TValue GetValueOrDefault<TValue>(string settingsKey, TValue defaultValue = default)
        {
            string value = Settings.GetValueOrDefault(settingsKey, null);

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