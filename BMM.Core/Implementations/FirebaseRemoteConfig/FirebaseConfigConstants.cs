namespace BMM.Core.Implementations.FirebaseRemoteConfig
{
    public class FirebaseConfigConstants
    {
        /* The default and recommended production fetch interval is 12 hours. If app fetches too many times in a short time period, fetch calls are throttled and SDK returns exception.
         * However during the development we might want to refresh cache very frequently to see changes done in FirebaseRemoteConfig console immediately.
         * For more information please see: https://firebase.google.com/docs/remote-config/use-config-ios#throttling */
    #if DEBUG
        public const long MinimumFetchIntervalInSeconds = 0L;
    #else
        public const long MinimumFetchIntervalInSeconds = 43200L;
    #endif
    }
}