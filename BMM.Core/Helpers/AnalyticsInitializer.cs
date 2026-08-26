using System.Net.Sockets;
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
            options.SampleRate = 0.25f;
            options.TracesSampleRate = 0.25;
            options.ProfilesSampleRate = 0.25;
            options.SetBeforeSend((sentryEvent, hint) =>
            {
                var ex = sentryEvent.Exception;
                if (ex is SocketException
                    || ex is System.Net.WebException
                    || ex?.Message?.Contains("Connection closed") == true
                    || ex?.GetType().Name == "JavaProxyThrowable")
                {
                    return null;
                }

                return sentryEvent;
            });
        }
    }
}