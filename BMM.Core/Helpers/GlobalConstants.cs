namespace BMM.Core.Helpers
{
    public class GlobalConstants
    {
        public const string PackageName = "org.brunstad.bmm";

        public const string ApiUrlDev = "https://devapibmm.brunstad.org"; // DEV Environment of the BMM API // It's not really used anymore
        public const string ApiUrlInt = "https://int-bmm-api.brunstad.org";
        public const string ApiUrlProd = "https://bmm-api.brunstad.org";

        public const string BmmUrlInt = "int-bmm.brunstad.org";
        public const string BmmUrlProd = "bmm.brunstad.org";

#if ENV_INT
        public const string ApiUrl = ApiUrlInt;
        public const string BmmUrl = BmmUrlInt;
#else
        public const string ApiUrl = ApiUrlProd;
        public const string BmmUrl = BmmUrlProd;
#endif

        // Used for translation files.
        public const string GeneralNamespace = "BMM";

        // The folder within Assets, where the translation-files are stored. These files MUST for Android be in the Assets/* folder and have the AssetAndroid build flag.
        public const string RootFolderForResources = "Translation";

        // Static list of default content languages which will be used if Firebase Remote Config can't connect to the server.
        public static readonly string DefaultContentLanguages = "af,bg,cs,de,el,en,es,et,fi,fr,he,hr,hu,it,nb,nl,pl,pt,ro,ru,sl,ta,tr,zh,zxx";

        // Static list of languages, this app has been translated into. That are folders within RootFolderForResources, wherein there is a JSON file per view-model, having the same name as the ViewModel.
        public static readonly string[] AvailableAppLanguages = { "af", "bg", "da", "de", "en", "es", "et", "fi", "fr", "hu", "it", "nb", "nl", "pl", "pt", "ro", "ru", "sl", "tr", "uk" };

        public const string ContactMailAddress = "bmm-support@bcc.no";

        public const string AppVersion = "DEV";

        public const string DefaultMinimumRequiredAndroidVersion = "0.0";
        public const string DefaultMinimumRequiredIosVersion = "0.0";

        public const string DefaultAndroidVersionPlannedToBeUnsupported = "4.4";
        public const string DefaultIosVersionPlannedToBeUnsupported = "0.0";

        public const string DefaultMinimumRequiredAppVersion = "1.0";
        public const string DefaultAppVersionPlannedToBeUnsupported = "1.0";

        public const int SearchHistoryCount = 10;

        public const string ApplicationName = "BMM";

        public const bool UseExternalStorageDefault = false;
        public const int LowStorageWarningValueDefault = 5;
        public const bool MobileDownloadEnabledDefault = false;
        public const bool PushNotificationsEnabledDefault = true;
        public const int NetworkRequestTimeout = 10;
        public const int DefaultNumberOfPodcastTracksToDownload = 3;
        public const string DroidAppSecret = "#{Android_APP_SECRET_PLACEHOLDER}#";
        public const string iOSAppSecret = "#{iOS_APP_SECRET_PLACEHOLDER}#";

        public const string AndroidUpdateLink = "https://play.google.com/store/apps/details?id=org.brunstad.bmm";
        public const string IosUpdateLink = "itms-apps://apps.apple.com/no/app/bmm-brunstad/id777577855";
    }
}