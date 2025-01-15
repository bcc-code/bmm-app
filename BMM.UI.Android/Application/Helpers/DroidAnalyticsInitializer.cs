using BMM.Core.Helpers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

namespace BMM.UI.Droid.Application.Helpers;

public class DroidAnalyticsInitializer
{
    public static void Init()
    {
        if (!AppCenter.Configured)
            AppCenter.Start(GlobalConstants.DroidAppSecret, typeof(Analytics));

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