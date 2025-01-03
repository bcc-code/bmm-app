using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.Maui.Devices;

namespace BMM.Core.Helpers
{
    public abstract class AnalyticsInitializer
    {
        private static string iOSPrefix = "ios_";
        private static string AndroidPrefix = "android_";
        private static string Dev = "dev";
        private static string Prod = "prod";
        
        public static void DroidRegister()
        {
            if (!AppCenter.Configured)
                AppCenter.Start(GlobalConstants.DroidAppSecret, typeof(Analytics));

            InitSentry();
        }

        public static void IOSRegister()
        {
            if (!AppCenter.Configured)
                AppCenter.Start(GlobalConstants.iOSAppSecret, typeof(Analytics));
                    
            InitSentry();
        }
        
        private static void InitSentry()
        {
            SentrySdk.Init(options =>
            {
                options.Dsn = GlobalConstants.SentryDsn;
                options.Environment = GetSentryEnvironment();
                options.Debug = false;
                options.TracesSampleRate = 1.0;
                options.ProfilesSampleRate = 1.0;
            });
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
    }
}