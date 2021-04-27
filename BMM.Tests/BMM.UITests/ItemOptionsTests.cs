using System;
using System.Threading.Tasks;
using BMM.UITests.Views;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class ItemOptionsTests
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;
        private string OptionNotVisible = "option is not visible.";

        public ItemOptionsTests(Platform platform)
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
        public async Task PlayerVisible_AssertAllTrackOptionsSuccessful()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.StartTrackWithinPlaylist();
            _app.Tap(_bmmApp.MiniPlayer.MiniPlayer);

            _app.Tap(_bmmApp.AudioPlayerPage.OptionsButton);

            AssertCancelOptionVisible(); 

            AssertAddToPlaylistOptionVisible();
            TestAddToPlaylistOption();
            BackToOptionsMenu();

            AssertAddToQueueOptionVisible();
            TestAddToQueueOption();
            _app.Tap(_bmmApp.AudioPlayerPage.OptionsButton);

            AssertGoToAlbumOptionVisible();
            TestGoToAlbumOption();
            BackToOptionsMenu();

            AssertGoToContributorsOptionVisible();
            TestGoToContributorsOption();
        }

        [Test]
        public async Task TrackCollectionVisible_AssertAllTrackOptionsVisible()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.StartTrackWithinPlaylist();
            _app.Tap(_bmmApp.MyContentPlaylistPage.OptionsButton(0));

            AssertCancelOptionVisible();
            AssertRemoveFromPlaylistOptionVisible(); 
            AssertAddToPlaylistOptionVisible();
            AssertAddToQueueOptionVisible();
            AssertGoToAlbumOptionVisible();
            AssertGoToContributorsOptionVisible();
            AssertMoreInformationOptionVisible();
        }

        [Test]
        public async Task TrackCollectionVisible_AssertMoreInformationCorrect()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.StartTrackWithinPlaylist();
            _app.Tap(_bmmApp.MiniPlayer.MiniPlayer);
            _app.Tap(_bmmApp.AudioPlayerPage.OptionsButton);
            AssertMoreInformationOptionVisible();
            TestMoreInformationOption();
        }

        [Test]
        public async Task TrackCollectionsVisible_AssertAllTrackCollectionOptionsVisible()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenMyContent(_app);
            _app.Tap(Menu.LabelMyContent);
            _app.Tap(_bmmApp.MyContentPlaylistPage.OptionsButton(0));
            AssertDeletePlaylistOptionVisible();
            AssertRenamePlaylistOptionVisible(); 
        }

        private void BackToOptionsMenu()
        {
            if(_app.Query(_bmmApp.MiniPlayer.MiniPlayer).Length ==0) 
                _app.Tap(_bmmApp.NavigationBar.BackButton);
            else
                _app.Tap(_bmmApp.MiniPlayer.MiniPlayer);
            _app.Tap(_bmmApp.AudioPlayerPage.OptionsButton);
        }

        private void AssertDeletePlaylistOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.DeletePlaylist,
                "Delete playlist option is not visible", TimeSpan.FromSeconds(3));
        }

        private void AssertRenamePlaylistOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.RenamePlaylist,
                "Rename playlist option is not visible", TimeSpan.FromSeconds(3));
        }

        private void AssertRemoveFromPlaylistOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.RemoveFromPlaylist,
                "Remove from playlist option is not visible", TimeSpan.FromSeconds(3));
        }

        private void AssertCancelOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.Cancel,
                "Cancel option is not visible", TimeSpan.FromSeconds(3));
        }

        private void AssertAddToPlaylistOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.AddToPlaylist,
                "Add to playlist " + OptionNotVisible, TimeSpan.FromSeconds(3));
        }

        private void TestAddToPlaylistOption()
        {
            _app.Tap(_bmmApp.OptionsPage.AddToPlaylist);
            _app.WaitForElement(_bmmApp.TrackCollectionAddToPage.TitleHeader, "Add to playlist header is not visible",
                TimeSpan.FromSeconds(3));
        }

        private void AssertAddToQueueOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.AddToQueue,
                "Add to queue " + OptionNotVisible, TimeSpan.FromSeconds(3));
        }

        private void TestAddToQueueOption()
        {
            _app.Tap(_bmmApp.OptionsPage.AddToQueue);
            _app.WaitForElement(c=>c.Marked("Added to queue"), "Item was not successfully added to the queue",
                TimeSpan.FromSeconds(3));
        }

        private void AssertGoToAlbumOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.GoToAlbum,
                "Go to album " + OptionNotVisible, TimeSpan.FromSeconds(3));
        }

        private void TestGoToAlbumOption()
        {
            _app.Tap(_bmmApp.OptionsPage.GoToAlbum);
            _app.WaitForElement(_bmmApp.AlbumPage.AlbumHeader, "Go to album page was not successful.",
                TimeSpan.FromSeconds(3));
        }

        private void AssertGoToContributorsOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.GoToContributors,
                  "Go to contributors " + OptionNotVisible, TimeSpan.FromSeconds(3));
        }

        private void TestGoToContributorsOption()
        {
            _app.Tap(_bmmApp.OptionsPage.GoToContributors);
            _app.WaitForElement(_bmmApp.GoToContributorPage.Performer, "Go to contributor page was not successful.",
                TimeSpan.FromSeconds(3));
            _app.Tap(_bmmApp.GoToContributorPage.Cancel);
        }

        private void AssertMoreInformationOptionVisible()
        {
            _app.WaitForElement(_bmmApp.OptionsPage.MoreInformation,
                "More information " + OptionNotVisible, TimeSpan.FromSeconds(3));
        }

        private void TestMoreInformationOption()
        {
            _app.Tap(_bmmApp.OptionsPage.MoreInformation);
            _app.WaitForElement(_bmmApp.MoreInformationPage.Title);
            var playlist = _bmmApp.MyContentPlaylistsPage.UiTestSamplePlaylist;

            string titleError = "Track Title is incorrect.";
            string expectedTitle = playlist.SecondTrackTitle;
            string actualTitle = _app.Query(_bmmApp.MoreInformationPage.Title)[0].Text;
            Assert.AreEqual(expectedTitle, actualTitle, titleError);

            string albumError = "Track Album is incorrect.";
            string expectedAlbum = playlist.SecondTrackAlbum;
            string actualAlbum = _app.Query(_bmmApp.MoreInformationPage.Album)[0].Text;
            Assert.AreEqual(expectedAlbum, actualAlbum, albumError);

            string artistError = "Track Artist is incorrect.";
            string actualArtist = _app.Query(_bmmApp.MoreInformationPage.Artist)[0].Text;
            Assert.AreEqual(expectedTitle, actualArtist, artistError);

            string durationError = "Track Duration is incorrect.";
            string expectedDuration = playlist.SecondTrackDuration;
            string actualDuration = _app.Query(_bmmApp.MoreInformationPage.Duration)[0].Text;
            Assert.AreEqual(expectedDuration, actualDuration, durationError);

            string publishDateError = "Track Title is incorrect.";
            string expectedPublishDate = playlist.SecondTrackPublishDate;
            string actualPublishDate = _app.Query(_bmmApp.MoreInformationPage.PublishDate)[0].Text;
            Assert.AreEqual(expectedPublishDate, actualPublishDate, publishDateError);
        }
    }
}
