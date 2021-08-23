using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Documents;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Test.Unit.GuardedActions.Base;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.Documents
{
    public class PostprocessDocumentsActionTests : GuardedActionWithParameterAndResultTests<
        PostprocessDocumentsAction,
        IEnumerable<Document>,
        IEnumerable<Document>>
    {
        private IListenedTracksStorage _listenedTracksStorageMock;

        protected override void PrepareMocks()
        {
            base.PrepareMocks();
            _listenedTracksStorageMock = Substitute.For<IListenedTracksStorage>();
        }

        protected override PostprocessDocumentsAction CreateAction()
        {
            return new(_listenedTracksStorageMock);
        }

        [Test]
        public async Task VideoDocumentsAreExcluded()
        {
            //Arrange
            var documents = new List<Track>
            {
                new()
                {
                    Subtype = TrackSubType.Video
                }
            };

            //Act
            var result = await GuardedAction.ExecuteGuarded(documents);

            //Assert
            result
                .Should()
                .BeEmpty();
        }

        [Test]
        public async Task IsListenedIsProperlyAssignedToTrack()
        {
            //Arrange
            var notListenedTrack = new Track()
            {
                Id = 1
            };

            var listenedTrack = new Track()
            {
                Id = 2
            };

            var documents = new List<Track>
            {
                notListenedTrack,
                listenedTrack
            };

            _listenedTracksStorageMock
                .TrackIsListened(notListenedTrack)
                .Returns(false);

            _listenedTracksStorageMock
                .TrackIsListened(listenedTrack)
                .Returns(true);

            //Act
            var result = await GuardedAction.ExecuteGuarded(documents);

            //Assert
            result
                .Should()
                .NotBeNullOrEmpty();

            result
                .OfType<Track>()
                .First(x => x.Id == notListenedTrack.Id)
                .IsListened
                .Should()
                .BeFalse();

            result
                .OfType<Track>()
                .First(x => x.Id == listenedTrack.Id)
                .IsListened
                .Should()
                .BeTrue();
        }
    }
}