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
		private const long DefaultLastPosition = 1000;
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

			_cacheWrapper
				.GetObject<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory)
				.Returns(listOfPlaybackHistoryEntries);

			//Act
			await Subject.AddPlayedTrack(mediaTrackMock, expectedLastPosition, DateTime.UtcNow);

			//Assert
			await _cacheWrapper
				.Received(1)
				.InsertObject(
					StorageKeys.PlaybackHistory,
					Arg.Is<List<PlaybackHistoryEntry>>(p => p.First().LastPosition == expectedLastPosition));
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

            _cacheWrapper
                .GetObject<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory)
                .Returns(listOfPlaybackHistoryEntries);

            //Act
            await Subject.AddPlayedTrack(mediaTrackMock, DefaultLastPosition, DateTime.UtcNow);

            //Assert
            await _cacheWrapper
                .Received(1)
                .InsertObject(
                    StorageKeys.PlaybackHistory,
                    Arg.Is<List<PlaybackHistoryEntry>>(list =>
                        list.Count == PlaybackHistoryService.MaxEntries
                        && list.Any(x => x.MediaTrack == mediaTrackMock)));
        }
    }
}