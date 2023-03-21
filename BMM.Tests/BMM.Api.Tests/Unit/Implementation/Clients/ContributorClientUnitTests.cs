using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Models;
using Moq;
using NUnit.Framework;

namespace BMM.Api.Test.Unit.Implementation.Clients
{
    [TestFixture]
    public class ContributorClientUnitTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Contributor>>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Track>>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<Contributor>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());
        }

        [Test]
        public async Task GetAll_ShouldReturnContributorListIfWeReceiveProperRespond()
        {
            // Arrange
            var apiInfoClient = new ContributorClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Contributor>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(CreateASampleListOfContributors()));

            // Act
            var result = await apiInfoClient.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateASampleListOfContributors().Count);
        }

        [Test]
        public async Task GetById_ShouldReturnContributorIfWeReceiveProperRespond()
        {
            // Arrange
            int id = 190;
            var apiInfoClient = new ContributorClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<Contributor>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(new Contributor() { Id = id }));

            // Act
            var result = await apiInfoClient.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, id);
        }

        [Test]
        public async Task GetByTerm_ShouldReturnContributorListIfWeReceiveProperRespond()
        {
            // Arrange
            var apiInfoClient = new ContributorClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Contributor>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(CreateASampleListOfContributors()));

            // Act
            var result = await apiInfoClient.GetByTerm("test Term");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateASampleListOfContributors().Count);
        }

        [Test]
        public async Task GetTracks_ShouldReturnTrackListIfWeReceiveProperRespond()
        {
            // Arrange
            var apiInfoClient = new ContributorClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Track>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(CreateASampleListOfTracks()));

            // Act
            var result = await apiInfoClient.GetTracks(1, default);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateASampleListOfTracks().Count);
        }

        [Test]
        public void Add_SuccessfullyCreated()
        {
            var testResponse = new HttpResponseMessage(HttpStatusCode.Created);
            testResponse.Headers.Location = new Uri("https://localhost/contributor/10001");
            testResponse.Headers.Add("X-Document-Id", "10001");
            
            RequestHandler
                .Setup(x => x.GetResponse(It.IsAny<IRequest>(), null, null))
                .Callback((IRequest request, IDictionary<string, string> customHeaders, CancellationToken? cancellationToken) =>
                {
                    Assert.AreEqual("https://localhost/contributor/", request.Uri.ToString());
                    Assert.AreEqual(0, request.Headers.Count);
                    Assert.IsInstanceOf(typeof(Contributor), request.Body);
                })
                .Returns(Task.FromResult<HttpResponseMessage>(testResponse));

            var client = new ContributorClient(RequestHandler.Object, new ApiBaseUri("https://localhost"), Logger.Object);

            var newContributor = new Contributor() { Name = "Foo Bar" };

            var result = client.Add(newContributor).Result;

            Assert.IsTrue(10001 == result);
        }

        private IList<Contributor> CreateASampleListOfContributors()
        {
            return new List<Contributor>() { new Contributor() };
        }

        private IList<Track> CreateASampleListOfTracks()
        {
            return new List<Track>() { new Track() };
        }
    }
}
