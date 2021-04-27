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
    public class PodcastClientTests : ClientTests
    {
        [SetUp]
        protected  override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Podcast>>(It.IsAny<IRequest>(), null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<Podcast>(It.IsAny<IRequest>(), null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Track>>(It.IsAny<IRequest>(), null))
                .Throws(new Exception());

        }

        [Test]
        public async Task GetAll_ShouldReturnPodcastsIfWeReceiveProperRespond()
        {
            // Arrange
            var podcastClient = new PodcastClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Podcast>>(It.IsAny<IRequest>(), null))
                .Returns(Task.FromResult(CreateASamplePodcasts()));

            // Act
            var result = await podcastClient.GetAll(CachePolicy.UseCacheAndRefreshOutdated);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateASamplePodcasts().Count);
        }

        [Test]
        public async Task GetById_ShouldReturnPodcastIfWeReceiveProperRespond()
        {
            // Arrange
            var id = 1;
            var podcastClient = new PodcastClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<Podcast>(It.IsAny<IRequest>(), null))
                .Returns(Task.FromResult(new Podcast(){Id = id }));

            // Act
            var result = await podcastClient.GetById(1, CachePolicy.UseCache);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, id);
        }

        [Test]
        public async Task GetTracks_ShouldReturnTracksIfWeReceiveProperRespond()
        {
            // Arrange
            var id = 1;
            var podcastClient = new PodcastClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Track>>(It.IsAny<IRequest>(), null))
                .Returns(Task.FromResult(CreateASampleListOfTracks()));

            // Act
            var result = await podcastClient.GetTracks(1, CachePolicy.UseCache);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateASampleListOfTracks().Count);
        }

        private IList<Podcast> CreateASamplePodcasts()
        {
            return new List<Podcast>() { new Podcast() };
        }

        private IList<Track> CreateASampleListOfTracks()
        {
            return new List<Track>() { new Track() };
        }
    }
}
