using BMM.Core.Helpers;

namespace BMM.UI.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            AnalyticsInitializer.IOSRegister();
            BmmApplication.Main(args, "BmmApplication", "AppDelegate");
        }
    }
}