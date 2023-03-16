using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Player;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Test.Unit.Base;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Plugin.Settings.Abstractions;

namespace BMM.Core.Test.Unit.Implementations.Player
{
    [TestFixture]
    public class PlaybackHistoryServiceTests : BaseTests<IPlaybackHistoryService>
    {
        private ISettings _settingsMock;
        private const int DefaultTrackId = 1;
        private const long DefaultLastPosition = 1000;

        protected override IPlaybackHistoryService CreateTestSubject()
        {
            return new PlaybackHistoryService();
        }

        protected override async Task PrepareMocks()
        {
            await base.PrepareMocks();
            _settingsMock = Substitute.For<ISettings>();
            AppSettings.SetImplementation(_settingsMock);
        }

        [Test]
        public async Task TrackShouldHaveUpdateLastPosition_WhenIsTheSameAsLastEntry()
        {
            //Arrange
            long expectedLastPosition = DefaultLastPosition * 2;

            var mediaTrackMock = new Track()
            {
                Id = DefaultTrackId
            };

            var listOfPlaybackHistoryEntries = new List<PlaybackHistoryEntry>
            {
                new PlaybackHistoryEntry(mediaTrackMock, DefaultLastPosition, DateTime.UtcNow)
            };

            _settingsMock
                .GetValueOrDefault(nameof(AppSettings.PlaybackHistory), null)
                .Returns(JsonConvert.SerializeObject(listOfPlaybackHistoryEntries));
                
            //Act
            await Subject.AddPlayedTrack(mediaTrackMock, expectedLastPosition, DateTime.UtcNow);

            //Assert
            _settingsMock
                .Received(1)
                .AddOrUpdateValue(
                    Arg.Is<string>(a => a == nameof(AppSettings.PlaybackHistory)),
                    Arg.Is<string>(v => CheckLastPositionItem(v, expectedLastPosition)));
        }

        private static bool CheckLastPositionItem(string v, long expectedLastPosition)
        {
            var deserialized = JsonConvert.DeserializeObject<List<PlaybackHistoryEntry>>(v);
            return deserialized.First().LastPosition == expectedLastPosition;
        }

        [Test]
        public async Task LastHistoryEntryIsRemoved_WhenMaxEntriesLimitIsExceeded()
        {
            //Arrange
            var mediaTrackMock = new Track()
            {
                Id = DefaultTrackId
            };

            var listOfPlaybackHistoryEntries = new List<PlaybackHistoryEntry>();

            for (int i = 0; i < PlaybackHistoryService.MaxEntries; i++)
                listOfPlaybackHistoryEntries.Add(new PlaybackHistoryEntry(Substitute.For<Track>(), DefaultLastPosition, DateTime.UtcNow));

            _settingsMock
                .GetValueOrDefault(nameof(AppSettings.PlaybackHistory), null)
                .Returns(JsonConvert.SerializeObject(listOfPlaybackHistoryEntries));

            //Act
            await Subject.AddPlayedTrack(mediaTrackMock, DefaultLastPosition, DateTime.UtcNow);

            //Assert
            _settingsMock
                .Received(1)
                .AddOrUpdateValue(
                    Arg.Is<string>(a => a == nameof(AppSettings.PlaybackHistory)),
                    Arg.Is<string>(v => CheckLastPositionItem(v, mediaTrackMock)));
        }
        
        private static bool CheckLastPositionItem(string v, IMediaTrack mediaTrack)
        {
            var deserialized = JsonConvert.DeserializeObject<List<PlaybackHistoryEntry>>(v);
            return deserialized.Count == PlaybackHistoryService.MaxEntries
                   && deserialized.Any(x => x.MediaTrack.Id == mediaTrack.Id);
        }
    }
}