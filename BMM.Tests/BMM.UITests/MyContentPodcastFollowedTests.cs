using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class MyContentPodcastFollowedTests
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;
        public const string  PodcastSection="Childrens MP3-favorites";
        public const string ErrorNoPodcast = "Podcast is not visible";
        public MyContentPodcastFollowedTests (Platform platform)
        {
            _platform = platform;
        }

        [SetUp]
        public void Setup()
        {
            _bmmApp = AppInitializer.StartApp(_platform);
            _app = _bmmApp.App;
        }

        [Test]
        public async Task FollowPodcast_AssertPodcastIsVisibleInFollowedPodcasts()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenSettings(_app);
            _app.Tap(_bmmApp.SettingsPage.DownloadViaMobileNetworkSwitch);

            _bmmApp.Menu.OpenLibrary(_app);
            _app.Tap(PodcastSection);
            _app.WaitForElement(_bmmApp.MyContentPodcastFollowedPage.FollowBtn);

            _app.Tap(_bmmApp.MyContentPodcastFollowedPage.FollowBtn);
            _bmmApp.Menu.OpenMyContent(_app);
            await Task.Delay(200);
            _app.WaitForElement(_bmmApp.MyContentPage.Podcasts, ErrorNoPodcast, TimeSpan.FromMilliseconds(200));
            _app.Tap(_bmmApp.MyContentPage.Podcasts);
            var errorFollowedNotFound = ""+ PodcastSection + " is not found";
            _app.WaitForElement(_bmmApp.MyContentPodcastFollowedPage.FollowedImage, errorFollowedNotFound, TimeSpan.FromSeconds(3));
        }
    }
}
