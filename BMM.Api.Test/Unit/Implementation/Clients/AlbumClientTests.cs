using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Models;
using Moq;
using NUnit.Framework;

namespace BMM.Api.Test.Unit.Implementation.Clients
{
    [TestFixture]
    public class AlbumClientTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<Album>(It.IsAny<IRequest>(), null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Album>>(It.IsAny<IRequest>(), null))
                .Throws(new Exception());
        }

        [Test]
        public async Task GetAll_ShouldReturnAlbumIfWeReceiveProperRespond()
        {
            // Arrange
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Album>>(It.IsAny<IRequest>(), null))
                .ReturnsAsync(CreateListOfTestAlbums());

            var albumClient = new AlbumClient(RequestHandler.Object, MockedUri, Logger.Object);

            // Act
            var result = await albumClient.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, CreateListOfTestAlbums().Count);
        }

        [Test]
        public async Task GetById_ShouldReturnAlbumIfWeReceiveProperRespond()
        {
            // Arrange
            var testID = "bmmtestId";
            RequestHandler.Setup(x => x.GetResolvedResponse<Album>(It.IsAny<IRequest>(), null))
                .ReturnsAsync(new Album(){ BmmId = testID });

            var albumClient = new AlbumClient(RequestHandler.Object, MockedUri, Logger.Object);

            // Act
            var result = await albumClient.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.BmmId, testID);
        }

        [Test]
        public async Task GetPublishedByYear_ShouldReturnAlbumIfWeReceiveProperRespond()
        {
            // Arrange
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Album>>(It.IsAny<IRequest>(), null))
                .ReturnsAsync(CreateListOfTestAlbums());

            var albumClient = new AlbumClient(RequestHandler.Object, MockedUri, Logger.Object);

            // Act
            var result = await albumClient.GetPublishedByYear(1999);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
        }

        [Test]
        public async Task GetRecordedByYear_ShouldReturnAlbumIfWeReceiveProperRespond()
        {
            // Arrange
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Album>>(It.IsAny<IRequest>(), null))
                .ReturnsAsync(CreateListOfTestAlbums());

            var albumClient = new AlbumClient(RequestHandler.Object, MockedUri, Logger.Object);

            // Act
            var result = await albumClient.GetRecordedByYear(1999);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
        }

        private IList<Album> CreateListOfTestAlbums()
        {
            return new List<Album>() {new Album() };
        }
    }
}
