using BMM.Core.Helpers;
using iOS.NewRelic;

namespace BMM.UI.iOS.Utils;

public class iOSAnalyticsInitializer
{
    public static void Init()
    {
        AnalyticsInitializer.IOSRegister();
        InitNewRelic();
    }

    private static void InitNewRelic()
    {
        if (GlobalConstants.NewRelic_iOSToken.Contains(GlobalConstants.Placeholder))
            return;
        
        NewRelic.SetPlatform(NRMAApplicationPlatform.Native);
        NewRelic.EnableCrashReporting(false);
        NewRelic.DisableFeatures(
            NRMAFeatureFlags.RequestErrorEvents
            | NRMAFeatureFlags.ExperimentalNetworkingInstrumentation
            | NRMAFeatureFlags.DefaultInteractions
            | NRMAFeatureFlags.SwiftAsyncURLSessionSupport
            | NRMAFeatureFlags.HttpResponseBodyCapture
            | NRMAFeatureFlags.AppStartMetrics
            | NRMAFeatureFlags.NetworkRequestEvents
            | NRMAFeatureFlags.InteractionTracing
            | NRMAFeatureFlags.FedRampEnabled
            | NRMAFeatureFlags.HandledExceptionEvents
            | NRMAFeatureFlags.NewEventSystem
            | NRMAFeatureFlags.WebViewInstrumentation
            | NRMAFeatureFlags.GestureInstrumentation
            | NRMAFeatureFlags.DistributedTracing
            | NRMAFeatureFlags.NSURLSessionInstrumentation
            | NRMAFeatureFlags.SwiftInteractionTracing);
        NewRelic.EnableFeatures(
            NRMAFeatureFlags.OfflineStorage);
        NRLogger.SetLogLevels((uint)NRLogLevels.Info);
        NewRelic.StartWithApplicationToken(GlobalConstants.NewRelic_iOSToken);
    }
}