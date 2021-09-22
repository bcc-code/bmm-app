using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.PlaybackHistory;
using BMM.Core.GuardedActions.PlaybackHistory.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Models.PlaybackHistory.Interfaces;
using BMM.Core.Test.Unit.GuardedActions.Base;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.PlaybackHistory
{
    [TestFixture]
    public class PreparePlaybackHistoryActionTests : GuardedActionWithResultTestBase<IPreparePlaybackHistoryAction, IEnumerable<Document>>
    {
        private const long DefaultLastPosition = 1000;

        private IPlaybackHistoryService _playbackHistoryServiceMock;
        private IBMMLanguageBinder _bmmLanguageBinderMock;

        protected override void PrepareMocks()
        {
            base.PrepareMocks();
            _playbackHistoryServiceMock = Substitute.For<IPlaybackHistoryService>();
            _bmmLanguageBinderMock = Substitute.For<IBMMLanguageBinder>();
        }

        protected override IPreparePlaybackHistoryAction CreateAction()
        {
            return new PreparePlaybackHistoryAction(_playbackHistoryServiceMock, _bmmLanguageBinderMock);
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
                new PlaybackHistoryEntry(expectedThirdTrack, DefaultLastPosition, DateTime.UtcNow.AddDays(2)),
                new PlaybackHistoryEntry(expectedFirstTrack, DefaultLastPosition, DateTime.UtcNow),
                new PlaybackHistoryEntry(expectedSecondTrack, DefaultLastPosition, DateTime.UtcNow.AddDays(1))
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