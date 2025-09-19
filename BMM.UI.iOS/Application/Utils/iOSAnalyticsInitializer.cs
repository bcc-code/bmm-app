using BMM.Core.Helpers;

namespace BMM.UI.iOS.Utils;

public class iOSAnalyticsInitializer
{
    public static void Init()
    {
        AnalyticsInitializer.IOSRegister();
    }
}