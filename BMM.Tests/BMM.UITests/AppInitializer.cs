using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BMM.UITests.Views;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Utils;

namespace BMM.UITests
{
    public interface IBmmApp
    {
        IApp App { get; }
        IMusicListPage MusicListPage { get; set; }
        IMyContentPlaylistPage MyContentPlaylistPage { get; set; }
        IAudioPlayerPage AudioPlayerPage { get; set; }
        ILoginPage LoginPage { get; set; }
        IMenu Menu { get; set; }
        IMyContentPage MyContentPage { get; set; }
        IMyContentPodcastFollowedPage MyContentPodcastFollowedPage { get; set; }
        INavigationBar NavigationBar { get; set; }
        IMyContentPlaylistsPage MyContentPlaylistsPage { get; set; }
        ILibraryArchivePage LibraryArchivePage { get; set; }
        IPodcastPage PodcastPage { get; set; }
        ILibraryPodcastsPage LibraryPodcastsPage { get; set; }
        ISettingsPage SettingsPage { get; set; }
        IMiniPlayer MiniPlayer { get; set; }
        IAlbumPage AlbumPage { get; set; }
        ISearchPage SearchPage { get; set; }
        IConfirmOptionsPage ConfirmOptionsPage { get; set; }
        ITrackCollectionAddToPage TrackCollectionAddToPage { get; set; }
        IGoToContributorPage GoToContributorPage { get; set; }
        IMoreInformationPage MoreInformationPage { get; set; }
        IOptionsPage OptionsPage { get; set; }
        IQueuePage QueuePage { get; set; }
        IContentLanguagePage ContentLanguagePage { get; set; }
        IExplorePage ExplorePage { get; set; }
        IBrowsePage BrowsePage { get; set; }
        Task LoginToApp();
        void StartTrackWithinPlaylist();
        Task OpenFraKaare();
    }

    public class BmmApp : IBmmApp
    {
        private const string FraaKareLink = "https://bmm.brunstad.org/playlist/podcast/1"; 
        private const int DelayAfterOpeningDeepLink = 500;
        private const string GoToLinkMethodName = "GoToLink";
        private string GoToLinkPlatformMethodName => Platform == Platform.Android ? GoToLinkMethodName : $"{GoToLinkMethodName}:"; 
        public IApp App { get; set; }
        public IMusicListPage MusicListPage { get; set; }
        public IMyContentPlaylistPage MyContentPlaylistPage { get; set; }
        public IAudioPlayerPage AudioPlayerPage { get; set; }
        public ILoginPage LoginPage { get; set; }
        public IMenu Menu { get; set; }
        public IMyContentPage MyContentPage { get; set; }
        public IMyContentPodcastFollowedPage MyContentPodcastFollowedPage { get; set; }
        public INavigationBar NavigationBar { get; set; }
        public IMyContentPlaylistsPage MyContentPlaylistsPage { get; set; }
        public ILibraryArchivePage LibraryArchivePage { get; set; }
        public IPodcastPage PodcastPage { get; set; }
        public ILibraryPodcastsPage LibraryPodcastsPage { get; set; }
        public ISettingsPage SettingsPage { get; set; }
        public IMiniPlayer MiniPlayer { get; set; }
        public IAlbumPage AlbumPage { get; set; }
        public ISearchPage SearchPage { get; set; }
        public IConfirmOptionsPage ConfirmOptionsPage { get; set; }
        public ITrackCollectionAddToPage TrackCollectionAddToPage { get; set; }
        public IGoToContributorPage GoToContributorPage { get; set; }
        public IMoreInformationPage MoreInformationPage { get; set; }
        public IOptionsPage OptionsPage { get; set; }
        public IQueuePage QueuePage { get; set; }
        public IExplorePage ExplorePage { get; set; }
        public IContentLanguagePage ContentLanguagePage { get; set; }
        public IBrowsePage BrowsePage { get; set; }
        public Platform Platform { get; set; }

        public async Task LoginToApp()
        {
            App.WaitForElement(LoginPage.WebView, "OIDC login webview not visible");

            App.WaitForElement(LoginPage.WebUserName);
            App.Tap(LoginPage.WebUserName);
            App.EnterText(TestSecrets.Username);
            App.DismissKeyboard();
            App.PinchToZoomOut(LoginPage.WebView);

            App.WaitForElement(LoginPage.WebPassword);
            App.Tap(LoginPage.WebPassword);
            App.EnterText(TestSecrets.LoginPassword);
            App.DismissKeyboard();
            App.PinchToZoomOut(LoginPage.WebView);

            App.ScrollDownTo(LoginPage.WebLoginButton, LoginPage.WebView);
            App.Tap(LoginPage.WebLoginButton);
            App.WaitForElement(Menu.BottomBar, "Bottom bar button not visible");
        }

        public void StartTrackWithinPlaylist()
        {
            Menu.OpenMyContent(App);
            App.Tap(MyContentPlaylistsPage.ItemWithTitle(MyContentPlaylistsPage.UiTestSamplePlaylist.Name));
            App.Tap(MyContentPlaylistPage.ItemWithTitle(MyContentPlaylistsPage.UiTestSamplePlaylist.SecondTrackTitle));
        }

        public async Task OpenFraKaare()
        {
            App.Invoke(GoToLinkPlatformMethodName, FraaKareLink);
            await Task.Delay(DelayAfterOpeningDeepLink);
        }
    }

    public class BmmWaitTimes : IWaitTimes
    {
        public TimeSpan WaitForTimeout => TimeSpan.FromMinutes(1);

        public TimeSpan GestureWaitTimeout => TimeSpan.FromMinutes(1);

        public TimeSpan GestureCompletionTimeout => TimeSpan.FromMinutes(1);
    }

    public static class AppInitializer
    {
        public static void LogCurrentTestName()
        {
            Console.WriteLine("Run Test: " + TestContext.CurrentContext.Test.FullName);
        }

        public static IBmmApp StartApp(Platform platform)
        {
            LogCurrentTestName();

            var platformApp = new BmmApp();
            platformApp.Platform = platform;

            if (platform == Platform.Android)
            {
                SetPageProperties(platformApp, "Android");

                platformApp.App = ConfigureApp
                    .Android
                    .InstalledApp("org.brunstad.bmm")
                    .EnableLocalScreenshots()
                    .WaitTimes(new BmmWaitTimes())
                    .StartApp();

                platformApp.App.Invoke("InvokeUiTestBrowser");
            }
            else if (platform == Platform.iOS)
            {
                SetPageProperties(platformApp, "Touch");

                platformApp.App = ConfigureApp
                    .iOS
                    .AutEnvironmentVars(new Dictionary<string, string> {{"USE_UI_TEST_OIDC_BROWSER", "1"}})
                    // Enable to test on a simulator device//
                    //.AppBundle(Configuration.PathToRoot + @"/BMM.UI.iOS/bin/iPhoneSimulator/Debug/BMM.app")
                    .InstalledApp("org.brunstad.bmm")
                    .EnableLocalScreenshots()
                    .WaitTimes(new BmmWaitTimes())
                    .StartApp();

                platformApp.App.Invoke("ClearAllLocalData");
            }

            return platformApp;
        }

        private static void SetPageProperties<TApp>(TApp app, string platform)
        {
            var appType = app.GetType();
            var assembly = appType.Assembly;
            var properties = appType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanWrite && x.PropertyType.IsInterface
                && x.PropertyType.Assembly.FullName == assembly.FullName);

            var pageTypes = new Dictionary<string, ConstructorInfo>(StringComparer.CurrentCultureIgnoreCase);

            foreach (
                var type in
                    assembly.GetTypes()
                        .Where(x => !x.Name.Contains("Test") && x.IsClass && !x.IsAbstract && !x.IsGenericType))
            {
                if (pageTypes.ContainsKey(type.Name))
                {
                    continue;
                }

                pageTypes.Add(type.Name, type.GetConstructor(Type.EmptyTypes));
            }

            foreach (var property in properties)
            {
                var platformPageName = string.Format("{0}{1}", platform, property.Name);

                if (pageTypes.ContainsKey(platformPageName))
                {
                    property.SetValue(app, pageTypes[platformPageName].Invoke(new object[0]));
                }
                else
                {
                    Assert.Fail("TestInitializer: The class " + platformPageName + " does not exist.");
                }
            }
        }
    }
}