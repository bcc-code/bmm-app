using System.Collections.Generic;
using BMM.Api.Framework;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.PlayObserver;
using BMM.Core.Implementations.Security;
using BMM.Core.Messages.MediaPlayer;
using Moq;
using MvvmCross.Plugin.Messenger;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.PlayObserver
{
    [TestFixture]
    public class PlayStatisticsTests
    {
        private PlayStatistics _playStatistics;
        private Mock<IMvxMessenger> _messenger;
        private Mock<IAnalytics> _analytics;
        private Mock<IUserStorage> _userStorage;
        private Mock<IStatisticsClient> _statisticsClient;
        private Mock<IPlaybackHistoryService> _playbackHistorySerivce;

        [SetUp]
        public void Init()
        {
            _messenger = new Mock<IMvxMessenger>();
            _analytics = new Mock<IAnalytics>();
            _userStorage = new Mock<IUserStorage>();
            _userStorage.Setup(storage => storage.GetUser()).Returns(new User {PersonId = 1111});
            _statisticsClient = new Mock<IStatisticsClient>();
            _playbackHistorySerivce = new Mock<IPlaybackHistoryService>();
            _playStatistics = new PlayStatistics(_analytics.Object,
                _userStorage.Object,
                new Mock<ILogger>().Object,
                new MeasurementCalculator(),
                _statisticsClient.Object,
                new ThreadBlockingExceptionHandler(),
                _messenger.Object,
                new Mock<IFirebaseRemoteConfig>().Object,
                _playbackHistorySerivce.Object);
            _playStatistics.TriggerClear = () => { _playStatistics.Clear(); };
        }

        [Test]
        public void StartingAndPausingAddsPortionListened()
        {
            // Arrange
            var currentPortions = _playStatistics.PortionsListened.Count;

            // Act
            _playStatistics.OnPlaybackStateChanged(new PlaybackState {IsPlaying = true});
            _playStatistics.OnPlaybackStateChanged(new PlaybackState {IsPlaying = false, CurrentPosition = 5000});

            // Assert
            Assert.AreEqual(currentPortions + 1, _playStatistics.PortionsListened.Count);
        }

        [Test]
        public void SeekingAddsPortion()
        {
            // Arrange
            var firstTrack = new CurrentTrackChangedMessage(CreateTrack(1), this);
            var secondTrack = new CurrentTrackChangedMessage(CreateTrack(2), this);

            // Act
            _playStatistics.OnCurrentTrackChanged(firstTrack);
            _playStatistics.OnSeeked(4000, 10000);

            // Assert
            Assert.AreEqual(1, _playStatistics.PortionsListened.Count);
        }

        [Test]
        public void CorrectOrder_LogsTrackPlayed()
        {
            _playStatistics.OnCurrentTrackChanged(new CurrentTrackChangedMessage(CreateTrack(1), this));
            _playStatistics.OnTrackCompleted(new TrackCompletedMessage(this) { });
            _analytics.Verify(x => x.LogEvent(It.Is<string>(s => s == "Track played"),
                It.Is<IDictionary<string, object>>(d => (int)d["trackId"] == 1)));
        }

        [Test]
        public void TestSeekEvents_SimulateTrackWithStartTime()
        {
            _playStatistics.OnCurrentTrackChanged(new CurrentTrackChangedMessage(CreateTrack(1), this));
            _playStatistics.OnSeeked(100, 0);
            _playStatistics.OnSeeked(0, 5);

            Assert.AreEqual(1, _playStatistics.PortionsListened.Count);
            Assert.AreEqual(5, _playStatistics.StartOfNextPortion);
        }

        public Track CreateTrack(int id)
        {
            return new Track
            {
                Tags = new List<string>(),
                Id = id, Media = new List<TrackMedia>
                {
                    new TrackMedia
                    {
                        IsVisible = true,
                        Type = TrackMediaType.Audio,
                        Files = new List<TrackMediaFile>
                        {
                            new TrackMediaFile {Duration = 10}
                        }
                    }
                }
            };
        }
    }
}
