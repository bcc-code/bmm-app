using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.Maui.Devices;

namespace BMM.Core.Helpers
{
    public abstract class AnalyticsInitializer
    {
        private const string iOSPrefix = "ios_";
        private const string AndroidPrefix = "android_";
        private const string Dev = "dev";
        private const string Prod = "prod";

        public static bool ShouldInitSentry => !GlobalConstants.SentryDsn.Contains(GlobalConstants.Placeholder); 
        
        public static void IOSRegister()
        {
            if (!AppCenter.Configured)
                AppCenter.Start(GlobalConstants.iOSAppSecret, typeof(Analytics));
                    
            if (ShouldInitSentry)
                SentrySdk.Init(SetupSentry);
        }
        
        private static string GetSentryEnvironment()
        {
            string env;
            
#if ENV_INT
            env = Dev;
#else
            env = Prod;
#endif

            return DeviceInfo.Current.Platform == DevicePlatform.Android
                ? $"{AndroidPrefix}{env}"
                : $"{iOSPrefix}{env}";
        }

        public static void SetupSentry(SentryOptions options)
        {
            options.Dsn = GlobalConstants.SentryDsn;
            options.Environment = GetSentryEnvironment();
            options.Debug = false;
            options.TracesSampleRate = 1.0;
            options.ProfilesSampleRate = 1.0;
        }
    }
}