using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class AudioPlayerTests
    {
        private IBmmApp _bmmApp;
        private IApp _app;
        private readonly Platform _platform;
        string expectedAppBarSubTitle = "1 of 2";
        private string _actualAppBarSubTitle;
        int delayShort = 500;
        int delayLong = 300;
        int delayExtraLong = 5000;

        public AudioPlayerTests(Platform platform)
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
        public async Task AfterTrackSelected_OpenPlayer_AssertPlayerAndTrackInfoVisible()
        {
            await LoginStartTrackOpenPlayer();
            AssertAllPlayerElementsVisible();
            AssertPlayerAndTrackInfoCorrect();
        }

        [Test]
        public async Task AfterTrackSelected_OpenPlayer_AssertPlayerFunctionality()
        {
            await LoginStartTrackOpenPlayer();
            await AssertTrackLoaded();
            await TestPreviousAfterDelay();
            await TestPreviousAndNextNoDelay();
            await TestRepeat();
            await TestPlayPause();
            await TestOpenClosePlayer();
            TestOpenCloseQueue();
        }

        [Test]
        public async Task AfterTrackSelected_OpenPlayer_AssertNoRepeatFunctionality()
        {
            await LoginStartTrackOpenPlayer();
            await Task.Delay(delayLong);
            await TestNoRepeat();
        }

        private async Task LoginStartTrackOpenPlayer()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.StartTrackWithinPlaylist();

            _app.WaitForElement(_bmmApp.MiniPlayer.MiniPlayer, "Mini player is not visible.", TimeSpan.FromSeconds(15));
            _app.Tap(_bmmApp.MiniPlayer.MiniPlayer);
        }

        private async  void AssertAllPlayerElementsVisible()
        {
            await Task.Delay(delayExtraLong);
           
            _app.WaitForElement(_bmmApp.AudioPlayerPage.OptionsButton);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.PlayPauseButton);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.PreviousButton);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.NextButton);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.RepeatButton);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.ShuffleButton);

            _app.WaitForElement(_bmmApp.AudioPlayerPage.Cover);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.SeekBar);

            _app.WaitForElement(_bmmApp.AudioPlayerPage.Player);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.PlayerAppBarSubTitleElement);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.PlayerAppBarTitleElement);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.PlayerTrackSubTitleElement);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.PlayerTrackTitleElement);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.TrackLength, "Track length not visible.", TimeSpan.FromSeconds(15));

            _app.WaitForElement(_bmmApp.AudioPlayerPage.OpenQueue, "Open Queue button not visible.", TimeSpan.FromSeconds(15));
        }

        private void AssertPlayerAndTrackInfoCorrect()
        {
            var trackTitle = _bmmApp.AudioPlayerPage.PlayerTrackTitleElement;
            var trackSubTitle = _bmmApp.AudioPlayerPage.PlayerTrackSubTitleElement;
            var appBarTitle = _bmmApp.AudioPlayerPage.PlayerAppBarTitleElement;

            var trackTitleError = "Player track title is not visible.";
            var trackSubTitleError = "Player track sub title is not visible.";
            var appBarTitleError = "Player appbar title is not visible.";
            var appBarSubTitleError = "Player appbar sub title is not visible.";
            var playlist = _bmmApp.MyContentPlaylistsPage.UiTestSamplePlaylist;

            _app.WaitForElement(_bmmApp.AudioPlayerPage.Player, "Player is not visible.", TimeSpan.FromSeconds(13));

            var actualTrackTitle = _app.Query(trackTitle)[0].Text;
            Assert.AreEqual(playlist.SecondTrackTitle, actualTrackTitle, trackTitleError);

            var actualTrackSubTitle = _app.Query(trackSubTitle)[0].Text;
            Assert.AreEqual(playlist.SecondTrackAlbum, actualTrackSubTitle, trackSubTitleError);

            var expectedAppBarTitle = playlist.SecondTrackAlbum;
            var actualAppBarTitle = _app.Query(appBarTitle)[0].Text;
            Assert.AreEqual(expectedAppBarTitle, actualAppBarTitle, appBarTitleError);

            _actualAppBarSubTitle = _app.Query(_bmmApp.AudioPlayerPage.PlayerAppBarSubTitleElement)[0].Text;
            Assert.AreEqual(expectedAppBarSubTitle, _actualAppBarSubTitle, appBarSubTitleError);
        }

        private async Task AssertTrackLoaded()
        {
            await Task.Delay(1500);
            var trackLoadingError = "Player did not load track successfully.";
            var expectedTrackLength = "9:45";
            _app.WaitForElement(_bmmApp.AudioPlayerPage.TrackLength);
            var actualTrackLength = _app.Query(_bmmApp.AudioPlayerPage.TrackLength)[0].Text;
            Assert.AreEqual(expectedTrackLength, actualTrackLength, trackLoadingError);
        }

        private async Task TestPreviousAfterDelay()
        {
            await Task.Delay(delayLong);
            _app.Tap(_bmmApp.AudioPlayerPage.PlayPauseButton);

            var TimeElapTxt = _app.Query(_bmmApp.AudioPlayerPage.TimeElapsed)[0].Text;
            if (TimeElapTxt == "00:00" || TimeElapTxt =="00:01" || TimeElapTxt == "00:02" || TimeElapTxt == "00:03")
            {
                _app.Tap(_bmmApp.AudioPlayerPage.PlayPauseButton);
                await Task.Delay(delayLong);
            }

            _app.Repl();
            _app.Tap(_bmmApp.AudioPlayerPage.PreviousButton);
            AssertPlayerAndTrackInfoCorrect();
        }

        private async Task TestPreviousAndNextNoDelay()
        {
            await Task.Delay(delayLong);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.PreviousButton);
            _app.Tap(_bmmApp.AudioPlayerPage.PlayPauseButton);
            _app.Tap(_bmmApp.AudioPlayerPage.PreviousButton);
            await Task.Delay(delayLong);
            _app.Tap(_bmmApp.AudioPlayerPage.PreviousButton);
            await Task.Delay(delayShort);
            _actualAppBarSubTitle = _app.Query(_bmmApp.AudioPlayerPage.PlayerAppBarSubTitleElement)[0].Text;
            Assert.AreNotEqual("2 of 2", _actualAppBarSubTitle, "Next or previous button is not working.");
            AssertPlayerAndTrackInfoCorrect();
        }

        private async Task TestNoRepeat()
        {
            _app.Tap(_bmmApp.AudioPlayerPage.NextButton);
            await Task.Delay(delayShort);
            _actualAppBarSubTitle = _app.Query(_bmmApp.AudioPlayerPage.PlayerAppBarSubTitleElement)[0].Text;
            _app.WaitForElement(_bmmApp.AudioPlayerPage.PlayerAppBarSubTitleElement, "Next button not visible", TimeSpan.FromSeconds(15));
            Assert.AreEqual("2 of 2", _actualAppBarSubTitle, "No repeat not working.");
        }

        private async Task TestRepeat()
        {
            _app.WaitForElement(_bmmApp.AudioPlayerPage.RepeatButton, "Repeat button not visible", TimeSpan.FromSeconds(15));
            _app.Tap(_bmmApp.AudioPlayerPage.RepeatButton);
            await Task.Delay(delayShort);
            _app.Tap(_bmmApp.AudioPlayerPage.NextButton);
            await Task.Delay(delayShort);
            _actualAppBarSubTitle = _app.Query(_bmmApp.AudioPlayerPage.PlayerAppBarSubTitleElement)[0].Text;
            Assert.AreNotEqual(expectedAppBarSubTitle, _actualAppBarSubTitle, "Repeat not working.");
            _app.Tap(_bmmApp.AudioPlayerPage.NextButton);
        }

        private async Task TestPlayPause()
        {
            await Task.Delay(delayLong);
            _app.Tap(_bmmApp.AudioPlayerPage.PlayPauseButton);
            var initialPosition = _app.Query(_bmmApp.AudioPlayerPage.TimeElapsed)[0].Text;
            await Task.Delay(delayShort);
            _app.Tap(_bmmApp.AudioPlayerPage.PlayPauseButton);
            await Task.Delay(delayLong);
            _app.Tap(_bmmApp.AudioPlayerPage.PlayPauseButton);
            var finalPosition = _app.Query(_bmmApp.AudioPlayerPage.TimeElapsed)[0].Text;
          //  if (initialPosition == finalPosition)
          //      Assert.Ignore("BUG: both tracks have 00:00 in their timlines");
          //  else
          //  {
                Assert.AreNotEqual(initialPosition, finalPosition, "Play/Pause button not working.");
        //    }
        }

        private async Task TestOpenClosePlayer()
        {
            await Task.Delay(1000);

            if (_app.Query(_bmmApp.AudioPlayerPage.ClosePlayerButton).ToString().Length>0)
            {
                _app.Tap(_bmmApp.AudioPlayerPage.ClosePlayerButton);
            }
            else
            {
                _app.Back();
            }

            _app.WaitForElement(_bmmApp.AudioPlayerPage.ClosePlayerButton, "Close player button not visible.", TimeSpan.FromSeconds(13));
            _app.Tap(_bmmApp.AudioPlayerPage.ClosePlayerButton); 
            _app.WaitForElement(_bmmApp.MiniPlayer.MiniPlayer);
            _app.Tap(_bmmApp.MiniPlayer.MiniPlayer);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.Player);
        }
        private void TestOpenCloseQueue()
        {
            _app.Tap(_bmmApp.AudioPlayerPage.OpenQueue);
            _app.WaitForElement(_bmmApp.QueuePage.Queue);
            _app.Tap(_bmmApp.QueuePage.CloseQueue);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.Player);
        }
    }
}
