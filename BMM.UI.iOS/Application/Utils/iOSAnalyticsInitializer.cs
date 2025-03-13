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
            | NRMAFeatureFlags.NetworkRequestEvents
            | NRMAFeatureFlags.InteractionTracing);
        NewRelic.EnableFeatures(
            NRMAFeatureFlags.OfflineStorage);
        NRLogger.SetLogLevels((uint)NRLogLevels.Info);
        NewRelic.StartWithApplicationToken(GlobalConstants.NewRelic_iOSToken);
    }
}