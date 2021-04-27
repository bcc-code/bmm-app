using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class MiniPlayerTests
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;

        public MiniPlayerTests(Platform platform)
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
        public async Task AfterTrackSelected_AssertMiniPlayerAndTrackInfoVisible()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.StartTrackWithinPlaylist();
            string miniPlayerError = "Miniplayer is not visible.";
            string TitleError = "Miniplayer track title is not visible.";
            string subTitleError = "Miniplayer track sub title is not visible.";
            _app.WaitForElement(_bmmApp.MiniPlayer.MiniPlayer, "Mini player is not visible.", TimeSpan.FromSeconds(3));

            var miniPlayer = _bmmApp.MiniPlayer.MiniPlayer;
            var subTitle = _bmmApp.MiniPlayer.MiniPlayerTrackSubTitleElement;
            var title = _bmmApp.MiniPlayer.MiniPlayerTrackTitleElement;

            _app.WaitForElement(miniPlayer, miniPlayerError, TimeSpan.FromSeconds(3));

            string expectedTitle = _bmmApp.MyContentPlaylistsPage.UiTestSamplePlaylist.SecondTrackTitle;
            string actualTitle = _app.Query(title)[0].Text;
            Assert.AreEqual(expectedTitle, actualTitle, TitleError);

            string expectedSubtitle = _bmmApp.MyContentPlaylistsPage.UiTestSamplePlaylist.SecondTrackAlbum;
            string actualSubtitle = _app.Query(subTitle)[0].Text;
            Assert.AreEqual(expectedSubtitle, actualSubtitle, subTitleError);
        }
    }
}
