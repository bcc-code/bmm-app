using Microsoft.Maui.ApplicationModel;

namespace BMM.Core.Support;

public abstract class OldSecureStorage : IOldSecureStorage
{
    // Special Alias that is only used for Secure Storage. All others should use: Preferences.GetPrivatePreferencesSharedName
    public static readonly string Alias = $"{AppInfo.PackageName}.xamarinessentials";

    public Task<string> GetAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        return PlatformGetAsync(key);
    }

    public Task SetAsync(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return PlatformSetAsync(key, value);
    }

    public bool Remove(string key) => PlatformRemove(key);
    public void RemoveAll() => PlatformRemoveAll();

    protected abstract Task<string> PlatformGetAsync(string key);
    protected abstract Task PlatformSetAsync(string key, string value);
    protected abstract bool PlatformRemove(string key);
    protected abstract void PlatformRemoveAll();
}