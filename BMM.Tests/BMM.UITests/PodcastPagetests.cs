using System;
using NUnit.Framework;
using Xamarin.UITest;
using System.Threading.Tasks;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class PodcastPageTests
    {
        private const string ErrorFromKaareTitleNotFound = "From Kåre is not found";
        private const string ErrorFollowNotFound = "Follow is not found";
        private const string ErrorImageNotFound = "Image is not found";
        private const string ErrorTracksNotFound = "Tracks are not found";

        private IBmmApp _bmmApp;
        private IApp _app;
        private readonly Platform _platform;

        public PodcastPageTests(Platform platform)
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
        public async Task OpenPodcast_AssertAllElementsAreVisible()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenLibrary(_app);
            _app.WaitForElement(_bmmApp.LibraryPodcastsPage.BtnFraKare, ErrorFromKaareTitleNotFound, TimeSpan.FromSeconds(5));
            _app.Tap("From Kåre");
            _app.WaitForElement(_bmmApp.PodcastPage.Follow);

            _app.WaitForElement(_bmmApp.PodcastPage.Title, ErrorFromKaareTitleNotFound);

            await Task.Delay(1500);
            if (_app.Query(_bmmApp.PodcastPage.Follow).Length == 0)
            {
                _app.WaitForElement(_bmmApp.PodcastPage.Following, ErrorFollowNotFound, TimeSpan.FromSeconds(5));
            }

            _app.WaitForElement(_bmmApp.PodcastPage.TrackTitle, "Tracks are not visible.", TimeSpan.FromSeconds(5));
            var trackTitles = _app.Query(_bmmApp.PodcastPage.TrackTitle);
            Assert.IsNotEmpty(trackTitles, ErrorTracksNotFound);
            Assert.IsNotEmpty(_app.Query(_bmmApp.PodcastPage.CoverImage), ErrorImageNotFound);
        }

        [Test]
        public async Task FollowingPodcast_StartsDownloadingTracks()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenLibrary(_app);
            _app.Tap(_bmmApp.LibraryPodcastsPage.BtnFraKare);
            _app.Tap(_bmmApp.PodcastPage.Follow);

            _app.WaitForElement(_bmmApp.PodcastPage.Following);
            var downloadImages = _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage);
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.AreEqual(3, downloadImages.Length);
        }
    }
}