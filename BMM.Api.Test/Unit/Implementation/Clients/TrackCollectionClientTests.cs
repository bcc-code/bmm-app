using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Models;
using Moq;
using NUnit.Framework;

namespace BMM.Api.Test.Unit.Implementation.Clients
{
    [TestFixture]
    public class TrackCollectionClientTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<TrackCollection>>(It.IsAny<IRequest>(), null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<TrackCollection>(It.IsAny<IRequest>(), null))
                .Throws(new Exception());
        }

        [Test]
        public async Task GetAll_ShouldReturnListOfTrackCollectionIfWeReceiveProperRespond()
        {
            // Arrange
            var trackCollectionClient = new TrackCollectionClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<TrackCollection>>(It.IsAny<IRequest>(), null))
                .Returns(Task.FromResult(CreateSampleTrackCollection()));

            // Act
            var result = await trackCollectionClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateSampleTrackCollection().Count);
        }

        [Test]
        public async Task GetGlobalDownloadedMost_ShouldReturnTrackCollectionIfWeReceiveProperRespond()
        {
            // Arrange
            int id = 100;
            var trackCollectionClient = new TrackCollectionClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<TrackCollection>(It.IsAny<IRequest>(), null))
                .Returns(Task.FromResult(new TrackCollection(){ Id = id}));

            // Act
            var result = await trackCollectionClient.GetById(id, CachePolicy.UseCacheAndRefreshOutdated);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, id);
        }

        private IList<TrackCollection> CreateSampleTrackCollection()
        {
            return new List<TrackCollection>(){ new TrackCollection() };
        }
    }
}
