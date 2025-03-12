using BMM.Core.Helpers;

namespace BMM.UI.Droid.Application.Helpers;

public class DroidAnalyticsInitializer
{
    public static void Init()
    {
        // CrossNewRelic.Current.Start(GlobalConstants.NewRelicDroidToken, new AgentStartConfiguration(
        //     crashReportingEnabled: false,
        //     loggingEnabled: false,
        //     networkRequestEnabled: false));
        
        InitSentry();
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