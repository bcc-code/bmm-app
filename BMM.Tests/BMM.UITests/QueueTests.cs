using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)] 
    public class QueueTests : TestBase
    {
        public QueueTests(Platform platform) : base(platform)
        {
        }

        [Test]
        public async Task AddTrackToQueue_AssertQueueContainsAllTrack()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.StartTrackWithinPlaylist();
            AddTrackToQueue();
            await Task.Delay(5000);

            _app.Tap(_bmmApp.MiniPlayer.MiniPlayer);
            AssertPlayerViewSubTitle();

            _app.Tap(_bmmApp.AudioPlayerPage.OpenQueue);
            AssertQueueELementsVisisble();
            AssertQueueContainsAllTracks();

            _app.Tap(_bmmApp.QueuePage.CloseQueue);
            _app.WaitForElement(_bmmApp.AudioPlayerPage.Player);
        }

        private void AddTrackToQueue()
        {
            _app.Tap(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle);
            _app.Tap(_bmmApp.OptionsPage.AddToQueue);
            _app.WaitForElement(c => c.Marked("Added to queue"), "Item was not successfully added to the queue",
                TimeSpan.FromSeconds(6));
        }

        private void AssertPlayerViewSubTitle()
        {
            var expectedAppBarSubTitle = "2 of 3";
            var actualAppBarSubTitle = _app.Query(_bmmApp.AudioPlayerPage.PlayerAppBarSubTitleElement)[0].Text;
            Assert.AreEqual(expectedAppBarSubTitle, actualAppBarSubTitle, "Track not added successfully to the queue");
        }

        private void AssertQueueELementsVisisble()
        {
            _app.WaitForElement(_bmmApp.QueuePage.CloseQueue);
            _app.WaitForElement(_bmmApp.QueuePage.Queue);
        }

        private void AssertQueueContainsAllTracks()
        {
            _app.WaitForElement(_bmmApp.QueuePage.OptionsButton(0));
            _app.WaitForElement(_bmmApp.QueuePage.OptionsButton(1));
            _app.WaitForElement(_bmmApp.QueuePage.OptionsButton(2), "Track previously added to queue is not visible.",
                TimeSpan.FromSeconds(3));
            _app.WaitForNoElement(_bmmApp.QueuePage.OptionsButton(3));
        }
    }
}
