using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.PlaybackHistory;
using BMM.Core.GuardedActions.PlaybackHistory.Interfaces;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.Test.Unit.GuardedActions.Base;
using BMM.Core.ViewModels.Interfaces;
using FluentAssertions;
using MvvmCross.Commands;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.PlaybackHistory
{
    [TestFixture]
    public class PreparePlaybackHistoryActionTests : GuardedActionWithResultTestBase<IPreparePlaybackHistoryAction, IEnumerable<IDocumentPO>>
    {
        private const long DefaultLastPosition = 1000;

        private IPlaybackHistoryService _playbackHistoryServiceMock;
        private IBMMLanguageBinder _bmmLanguageBinderMock;
        private IDocumentsPOFactory _documentsPOFactoryMock;
        private IPlaybackHistoryViewModel _dataContextMock;

        protected override void PrepareMocks()
        {
            base.PrepareMocks();
            _playbackHistoryServiceMock = Substitute.For<IPlaybackHistoryService>();
            _bmmLanguageBinderMock = Substitute.For<IBMMLanguageBinder>();
            _documentsPOFactoryMock = Substitute.For<IDocumentsPOFactory>();
            
            _documentsPOFactoryMock
                .Create(
                    Arg.Any<IEnumerable<Document>>(),
                    Arg.Any<IMvxCommand<IDocumentPO>>(),
                    Arg.Any<IMvxAsyncCommand<Document>>(),
                    Arg.Any<ITrackInfoProvider>())
                .Returns(arg =>
                {
                    var listToReturn = new List<IDocumentPO>();
                    var documentList = (IEnumerable<Document>)arg[0];

                    foreach (var document in documentList)
                    {
                        if (document is not Track track)
                            continue;
                        
                        var trackPO = Substitute.For<ITrackPO>();
                        trackPO
                            .Track
                            .Returns(track);
                        listToReturn.Add(trackPO);
                    }

                    return listToReturn;
                });
            
            _dataContextMock = Substitute.For<IPlaybackHistoryViewModel>();
        }

        protected override IPreparePlaybackHistoryAction CreateAction()
        {
            var preparePlaybackHistoryAction = new PreparePlaybackHistoryAction(
                _playbackHistoryServiceMock,
                _bmmLanguageBinderMock,
                _documentsPOFactoryMock);
            preparePlaybackHistoryAction.AttachDataContext(_dataContextMock);
            return preparePlaybackHistoryAction;
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
                .OfType<ITrackPO>()
                .ToList();
            
            tracks[0]
                .Track
                .Should()
                .Be(expectedFirstTrack);
            
            tracks[1]
                .Track
                .Should()
                .Be(expectedSecondTrack);
            
            tracks[2]
                .Track
                .Should()
                .Be(expectedThirdTrack);
        }
    }
}