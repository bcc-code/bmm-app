using BMM.Core.Helpers;
using Com.Newrelic.Agent.Android;
using DroidNewRelic = Com.Newrelic.Agent.Android.NewRelic;

namespace BMM.UI.Droid.Application.Helpers;

public class DroidAnalyticsInitializer
{
    public static void Init()
    {
        InitNewRelic();
        InitSentry();
    }

    private static void InitNewRelic()
    {
        if (GlobalConstants.NewRelicAndroidToken.Contains(GlobalConstants.Placeholder))
            return;
        
        DroidNewRelic.DisableFeature(FeatureFlag.NetworkRequests);
        DroidNewRelic.DisableFeature(FeatureFlag.NetworkErrorRequests);
        DroidNewRelic.DisableFeature(FeatureFlag.HandledExceptions);
        DroidNewRelic.DisableFeature(FeatureFlag.DefaultInteractions);
        DroidNewRelic.DisableFeature(FeatureFlag.HttpResponseBodyCapture);
        DroidNewRelic.DisableFeature(FeatureFlag.AppStartMetrics);
        DroidNewRelic.DisableFeature(FeatureFlag.InteractionTracing);
        DroidNewRelic.DisableFeature(FeatureFlag.FedRampEnabled);
        DroidNewRelic.DisableFeature(FeatureFlag.NativeReporting);
        DroidNewRelic.DisableFeature(FeatureFlag.ApplicationExitReporting);
        DroidNewRelic.DisableFeature(FeatureFlag.Jetpack);
        DroidNewRelic.DisableFeature(FeatureFlag.DistributedTracing);
        DroidNewRelic.DisableFeature(FeatureFlag.CrashReporting);
        DroidNewRelic.DisableFeature(FeatureFlag.LogReporting);
        DroidNewRelic.EnableFeature(FeatureFlag.OfflineStorage);
        DroidNewRelic.EnableFeature(FeatureFlag.BackgroundReporting);
        
        var newRelic = DroidNewRelic.WithApplicationToken(GlobalConstants.NewRelicAndroidToken)
            !.WithLoggingEnabled(false)
            !.WithCrashReportingEnabled(false);
        
        newRelic!.Start(Android.App.Application.Context);
    }

    private static void InitSentry()
    {
        if (!AnalyticsInitializer.ShouldInitSentry) 
            return;
        
        SentrySdk.Init(options =>
        {
            AnalyticsInitializer.SetupSentry(options);
            options.Native.EnableNdk = false;
        });
    }
}