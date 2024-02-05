namespace BMM.Api.Implementation.Constants
{
    /// <summary>
    /// These strings are also used in the Api. Changing them means that the Api suddenly has to support 2 versions (because of backwards compatibility).
    /// So, in general you should avoid changing below strings and better have a really good reason.
    /// </summary>
    public static class HeaderNames
    {
        public const string AcceptLanguage = "Accept-Language";
        public const string AppConnectivity = "AppConnectivity";
        public const string BmmVersion = "BMM-Version";
        public const string ExperimentId = "ExperimentId";
        public const string Accept = "Accept";
        public const string MobileDownloadAllowed = "MobileDownloadAllowed";
        public const string Authorization = "Authorization";
        public const string UiLanguage = "UiLanguage";
    }
}