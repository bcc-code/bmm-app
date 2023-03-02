using System;
using System.Collections.Generic;
using BMM.Api.Framework.HTTP;
using Moq;
using System.Threading;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Net.Http;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Clients;
using System.Net;
using BMM.Api.Abstraction;
using BMM.Api.Implementation;

namespace BMM.Api.Test.Unit.Implementation.Clients
{
    public class TrackClientTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x =>  x.GetResolvedResponse<IList<Track>>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<Track>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());

            RequestHandler.Setup(x => x.GetResolvedResponse<TrackRaw>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());
        }

        [Test]
        public void Add_SuccessfullyCreated()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Location = new Uri("https://localhost/track/10001");
            response.Headers.Add("X-Document-Id", "10001");
            response.Content = new StringContent("content");
            response.Content.Headers.Add("X-Document-Id", "10001");

            RequestHandler
                .Setup(x => x.GetResponse(It.IsAny<IRequest>(), null, null))
                .Callback((IRequest request, CancellationToken? cancellationToken) =>
                {
                    Assert.AreEqual("https://localhost/track/", request.Uri.ToString());
                    Assert.AreEqual(0, request.Headers.Count);
                    Assert.IsInstanceOf(typeof(TrackRaw), request.Body);
                })
                .Returns(Task.FromResult(response));

            var client = new TracksClient(RequestHandler.Object, new ApiBaseUri("https://localhost"), Logger.Object);

            var newTrack = new TrackRaw() { ParentId = 5000 };

            var result = client.Add(newTrack).Result;

            Assert.True(10001 == result);
        }

        [Test]
        public void Save_SuccessfullyCreated()
        {
            RequestHandler
                .Setup(x => x.GetResponse(It.IsAny<IRequest>(), null, null))
                .Callback((IRequest request, CancellationToken? cancellationToken) =>
                {
                    Assert.AreEqual("https://localhost/track/2000", request.Uri.ToString());
                    Assert.AreEqual(0, request.Headers.Count);
                    Assert.IsInstanceOf(typeof(TrackRaw), request.Body);
                })
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent)));

            var client = new TracksClient(RequestHandler.Object, new ApiBaseUri("https://localhost"), Logger.Object);

            var newTrack = new TrackRaw() { Id = 2000, ParentId = 5000 };

            var result = client.Save(newTrack).Result;

            Assert.True(result);
        }

        [Test]
        public async Task GetAll_ShouldReturnListOfTracksIfWeReceiveProperRespond()
        {
            // Arrange
            var trackClient = new TracksClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Track>>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(CreateSampleTracks()));

            // Act
            var result = await trackClient.GetAll(CachePolicy.UseCache);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateSampleTracks().Count);
        }

        [Test]
        public void GetById_ShouldPassExceptionThrough()
        {
            // Arrange
            var trackClient = new TracksClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<Track>(It.IsAny<IRequest>(), null, null)).Throws<KeyNotFoundException>();

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => trackClient.GetById(1));
        }

        [Test]
        public async Task GetById_ShouldReturnTrackIfWeReceiveProperRespond()
        {
            // Arrange
            var id = 98910;
            var trackClient = new TracksClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<Track>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(new Track(){ Id = id}));

            // Act
            var result = await trackClient.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Id, id);
        }

        [Test]
        public async Task GetRelated_ShouldReturnTrackRawIfWeReceiveProperResponse()
        {
            // Arrange
            var trackClient = new TracksClient(RequestHandler.Object, MockedUri, Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<Track>>(It.IsAny<IRequest>(), null, null))
                .ReturnsAsync(CreateSampleTracks());

            // Act
            var result = await trackClient.GetRelated(new TrackRelation());

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateSampleTracks().Count);
        }

        private IList<Track> CreateSampleTracks()
        {
            return new List<Track> { new Track()};
        }

    }
}

