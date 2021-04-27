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
        private const string errorFromKaareNotFound = "From Kåre is not found";
        private const string expectedTitle = "From Kåre";
        private const string errorFollowNotFound = "Follow is not found";
        private const string errorNewEpisodesNotFound = "New episodes sub-title is not found";
        private int i = 0;
        private const string errorGetNotifiedNotFound = "Get notified sub-title is not found";
        private const string errorImageNotFound = "Image is not found";
        private int numberExpected = 0;
        private const string errorTracksNotFound = "Tracks are not found";


        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;

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
            _app.WaitForElement(_bmmApp.LibraryPodcastsPage.BtnFraKare, errorFromKaareNotFound, TimeSpan.FromSeconds(5));
            _app.Tap("From Kåre");
            _app.WaitForElement(_bmmApp.PodcastPage.Follow);

            string actualTitle = _app.Query(_bmmApp.PodcastPage.ItemWithTitle("From Kåre")) [0].Text;
            while (actualTitle != "From Kåre")
            {
                actualTitle = _app.Query(_bmmApp.PodcastPage.ItemWithTitle("From Kåre")) [i].Text;
                i = i + 1;
            }

            Assert.AreEqual(expectedTitle, actualTitle, errorFromKaareNotFound);

            string expectedGetnotified = "Get notified";
            string actualGetnotified = _app.Query(_bmmApp.PodcastPage.Getnotified)[0].Text;
            Assert.AreEqual(expectedGetnotified, actualGetnotified, errorGetNotifiedNotFound);

            string expectedSubtitle = "when new episodes are out";
            string actualsubtitle = _app.Query(_bmmApp.PodcastPage.Subsubtitle)[0].Text;
            Assert.AreEqual(expectedSubtitle, actualsubtitle, errorNewEpisodesNotFound);

            await Task.Delay(1500);
            if (_app.Query(_bmmApp.PodcastPage.Follow).Length == 0)
            {
                _app.WaitForElement(_bmmApp.PodcastPage.Following, errorFollowNotFound, TimeSpan.FromSeconds(5));
            }

            _app.WaitForElement(_bmmApp.PodcastPage.Title_track, "Tracks are not visible.", TimeSpan.FromSeconds(5));
            int actualNumber = _app.Query(_bmmApp.PodcastPage.Title_track).Length;
            Assert.AreNotEqual(actualNumber, numberExpected, errorTracksNotFound);
            Assert.IsNotEmpty(_app.Query(_bmmApp.PodcastPage.CoverImage), errorImageNotFound);
        }
    }
}
