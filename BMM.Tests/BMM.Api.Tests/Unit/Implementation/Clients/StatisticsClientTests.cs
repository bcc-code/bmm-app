using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Models;
using Moq;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Api.Test.Unit.Implementation.Clients
{
    [TestFixture]
    public class StatisticsClientTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());
        }

        [Test]
        public async Task GetGlobalDownloadedMost_ShouldReturnListOfElementsIfWeReceiveProperRespond()
        {
            // Arrange
            var statisticsClient = new StatisticsClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(GetASampleListOfDocuments()));

            // Act
            var result = await statisticsClient.GetGlobalDownloadedMost(DocumentType.Album);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, GetASampleListOfDocuments().Count);
        }

        [Test]
        public async Task GetGlobalDownloadedRecently_ShouldReturnListOfElementsIfWeReceiveProperRespond()
        {
            // Arrange
            var statisticsClient = new StatisticsClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(GetASampleListOfDocuments()));

            // Act
            var result = await statisticsClient.GetGlobalDownloadedRecently(DocumentType.Album);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, GetASampleListOfDocuments().Count);
        }

        [Test]
        public async Task GetGlobalViewedMost_ShouldReturnListOfElementsIfWeReceiveProperRespond()
        {
            // Arrange
            var statisticsClient = new StatisticsClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(GetASampleListOfDocuments()));

            // Act
            var result = await statisticsClient.GetGlobalViewedMost(DocumentType.Album);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, GetASampleListOfDocuments().Count);
        }

        [Test]
        public async Task GetGlobalViewedRecently_ShouldReturnListOfElementsIfWeReceiveProperRespond()
        {
            // Arrange
            var statisticsClient = new StatisticsClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(GetASampleListOfDocuments()));

            // Act
            var result = await statisticsClient.GetGlobalViewedRecently(DocumentType.Album);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, GetASampleListOfDocuments().Count);
        }

        [Test]
        public async Task GetUserDownloadedMost_ShouldReturnListOfElementsIfWeReceiveProperRespond()
        {
            // Arrange
            var statisticsClient = new StatisticsClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(GetASampleListOfDocuments()));

            // Act
            var result = await statisticsClient.GetUserDownloadedMost("user1",DocumentType.Album);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, GetASampleListOfDocuments().Count);
        }

        [Test]
        public async Task GetUserDownloadedRecently_ShouldReturnListOfElementsIfWeReceiveProperRespond()
        {
            // Arrange
            var statisticsClient = new StatisticsClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(GetASampleListOfDocuments()));

            // Act
            var result = await statisticsClient.GetUserDownloadedRecently("user1", DocumentType.Album);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, GetASampleListOfDocuments().Count);
        }

        [Test]
        public async Task GetUserViewedMost_ShouldReturnListOfElementsIfWeReceiveProperRespond()
        {
            // Arrange
            var statisticsClient = new StatisticsClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(GetASampleListOfDocuments()));

            // Act
            var result = await statisticsClient.GetUserViewedMost("user1", DocumentType.Album);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, GetASampleListOfDocuments().Count);
        }

        [Test]
        public async Task GetUserViewedRecently_ShouldReturnListOfElementsIfWeReceiveProperRespond()
        {
            // Arrange
            var statisticsClient = new StatisticsClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(GetASampleListOfDocuments()));

            // Act
            var result = await statisticsClient.GetUserViewedRecently("user1", DocumentType.Album);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, GetASampleListOfDocuments().Count);
        }

        private IList<Document> GetASampleListOfDocuments()
        {
            return new List<Document> { Substitute.For<Document>() };
        }
    }
}
