using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Player;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Test.Unit.Base;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.Player
{
	[TestFixture]
	public class PlaybackHistoryServiceTests : BaseTests<IPlaybackHistoryService>
	{
		private const int DefaultTrackId = 1;
		private ICache _cacheWrapper;

		protected override IPlaybackHistoryService CreateTestSubject()
		{
			return new PlaybackHistoryService(_cacheWrapper);
		}

		protected override Task PrepareMocks()
		{
            _cacheWrapper = Substitute.For<ICache>();
			return base.PrepareMocks();
		}

		[Test]
		public async Task TrackShouldNotBeAddedToHistory_WhenIsTheSameAsLastEntry()
		{
			//Arrange
			var mediaTrackMock = new Track()
			{
				Id = DefaultTrackId
			};

			var listOfPlaybackHistoryEntries = new List<PlaybackHistoryEntry>
			{
				new PlaybackHistoryEntry(mediaTrackMock, DateTime.UtcNow)
			};

			_cacheWrapper
				.GetObject<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory)
				.Returns(listOfPlaybackHistoryEntries);

			//Act
			await Subject.AddPlayedTrack(mediaTrackMock);

			//Assert
			await _cacheWrapper
				.DidNotReceive()
				.InsertObject(
					StorageKeys.PlaybackHistory,
					Arg.Any<List<PlaybackHistoryEntry>>());
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
                listOfPlaybackHistoryEntries.Add(new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow));

            _cacheWrapper
                .GetObject<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory)
                .Returns(listOfPlaybackHistoryEntries);

            //Act
            await Subject.AddPlayedTrack(mediaTrackMock);

            //Assert
            await _cacheWrapper
                .Received(1)
                .InsertObject(
                    StorageKeys.PlaybackHistory,
                    Arg.Is<List<PlaybackHistoryEntry>>(list =>
                        list.Count == PlaybackHistoryService.MaxEntries
                        && list.Any(x => x.MediaTrack == mediaTrackMock)));
        }

        [Test]
        public async Task HistoryIsRetrievedInCorrectOrder_WhenGetAllGetsCalled()
        {
            //Arrange
            var listOfPlaybackHistoryEntries = new List<PlaybackHistoryEntry>()
            {
                new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow.AddDays(2)),
                new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow),
                new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow.AddDays(1))
            };

            _cacheWrapper
                .GetObject<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory)
                .Returns(listOfPlaybackHistoryEntries);

            //Act
            var result = await Subject.GetAll();

            //Assert
            result
                .Should()
                .HaveCount(3)
                .And
                .BeInDescendingOrder(x => x.GroupDateTime);
        }

        [Test]
        public async Task HistoryIsRetrievedWithCorrectDaysSections_WhenMoreThanOneTrackIsAssignedToOneDay()
        {
            //Arrange
            var listOfPlaybackHistoryEntries = new List<PlaybackHistoryEntry>()
            {
                new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow.AddDays(2)),
                new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow),
                new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow.AddDays(1)),
                new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow.AddDays(1)),
                new PlaybackHistoryEntry(Substitute.For<Track>(), DateTime.UtcNow.AddDays(1)),
            };

            _cacheWrapper
                .GetObject<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory)
                .Returns(listOfPlaybackHistoryEntries);

            //Act
            var result = await Subject.GetAll();

            //Assert
            result
                .Should()
                .HaveCount(3)
                .And
                .BeInDescendingOrder(x => x.GroupDateTime);
        }
	}
}