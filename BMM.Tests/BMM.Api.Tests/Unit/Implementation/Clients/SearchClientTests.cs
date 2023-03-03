using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SearchClientTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<IList<string>>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());
        }

        [Test]
        public async Task GetAll_ShouldReturnPodcastsIfWeReceiveProperRespond()
        {
            // Arrange
            var searchClient = new SearchClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Document>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(CreateASampleListOfDocuments()));

            // Act
            var result = await searchClient.GetAll("Test Term");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Items.Count(), CreateASampleListOfDocuments().Count);
        }

        [Test]
        public async Task GetSuggestions_ShouldReturnSugestionsIfWeReceiveProperRespond()
        {
            // Arrange
            var searchClient = new SearchClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<string>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(CreateASampleListOfSugestions()));

            // Act
            var result = await searchClient.GetSuggestions("Test Sugestion");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateASampleListOfSugestions().Count);
        }

        private IList<Document> CreateASampleListOfDocuments()
        {
            return new List<Document>() { Substitute.For<Document>() };
        }

        private IList<string> CreateASampleListOfSugestions()
        {
            return new List<string>() {"Suggest 1", "Suggest 2"};
        }
    }
}
