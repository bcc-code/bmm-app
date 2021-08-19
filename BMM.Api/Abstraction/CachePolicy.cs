namespace BMM.Api.Abstraction
{
    public enum CachePolicy
    {
        /// <summary>
        /// Uses the cache but if the cache is outdated it immediately loads an update and returns that update.
        /// </summary>
        UseCacheAndWaitForUpdates,
        /// <summary>
        /// Uses the cache but if the cache is outdated it returns the outdated item and reloads a newer version in the background.
        /// </summary>
        UseCacheAndRefreshOutdated,
        UseCache,
        ForceGetAndUpdateCache,
        IgnoreCache
    }
}