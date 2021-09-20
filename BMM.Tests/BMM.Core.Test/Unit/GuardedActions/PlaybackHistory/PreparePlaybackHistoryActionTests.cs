using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.PlaybackHistory;
using BMM.Core.GuardedActions.PlaybackHistory.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Test.Unit.GuardedActions.Base;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.PlaybackHistory
{
    [TestFixture]
    public class PreparePlaybackHistoryActionTests : GuardedActionWithResultTestBase<IPreparePlaybackHistoryAction, IEnumerable<Document>>
    {
        private IPlaybackHistoryService _playbackHistoryServiceMock;

        protected override void PrepareMocks()
        {
            base.PrepareMocks();
            _playbackHistoryServiceMock = Substitute.For<IPlaybackHistoryService>();
        }

        protected override IPreparePlaybackHistoryAction CreateAction()
        {
            return new PreparePlaybackHistoryAction(_playbackHistoryServiceMock);
        }

        [TestCase]
        public async Task HistoryIsRetrievedInCorrectOrder_WhenActionIsCalled()
        {
            //Arrange
            var expectedFirstTrack = Substitute.For<Track>();
            var expectedSecondTrack = Substitute.For<Track>();
            var expectedThirdTrack = Substitute.For<Track>();

            var listOfPlaybackHistoryEntries = new List<PlaybackHistoryEntry>()
            {
                new PlaybackHistoryEntry(expectedThirdTrack, DateTime.UtcNow.AddDays(2)),
                new PlaybackHistoryEntry(expectedFirstTrack, DateTime.UtcNow),
                new PlaybackHistoryEntry(expectedSecondTrack, DateTime.UtcNow.AddDays(1))
            };

            _playbackHistoryServiceMock
                .GetAll()
                .Returns(listOfPlaybackHistoryEntries);

            //Act
            var result = await GuardedAction.ExecuteGuarded();

            //Assert
            var tracks = result
                .OfType<Track>()
                .ToList();

            tracks[0] = expectedFirstTrack;
            tracks[1] = expectedSecondTrack;
            tracks[2] = expectedThirdTrack;
        }
    }
}