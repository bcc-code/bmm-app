using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.Documents;
using BMM.Core.Implementations.TrackListenedObservation;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.Test.Unit.GuardedActions.Base;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.GuardedActions.Documents
{
    [TestFixture]
    public class PostprocessDocumentsActionTests : GuardedActionWithParameterAndResultTests<
        PostprocessDocumentsAction,
        IEnumerable<IDocumentPO>,
        IEnumerable<IDocumentPO>>
    {
        protected override PostprocessDocumentsAction CreateAction()
        {
            return new PostprocessDocumentsAction();
        }

        [Test]
        public async Task VideoDocumentsAreExcluded()
        {
            //Arrange
            var trackPOSubstitute = Substitute.For<ITrackPO>();
            var track = new Track()
            {
                Subtype = TrackSubType.Video
            };

            trackPOSubstitute
                .Track
                .Returns(track);
            
            var documents = new List<ITrackPO>
            {
                trackPOSubstitute
            };

            //Act
            var result = await GuardedAction.ExecuteGuarded(documents);

            //Assert
            result
                .Should()
                .BeEmpty();
        }
    }
}