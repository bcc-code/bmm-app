using BMM.Core.Helpers;
using BMM.UI.iOS.Utils;

namespace BMM.UI.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            iOSAnalyticsInitializer.Init();
            BmmApplication.Main(args, "BmmApplication", "AppDelegate");
        }
    }
}