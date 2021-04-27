using System;
using System.Collections.Generic;
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
    public class FacetsClientTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<DocumentYear>>(It.IsAny<IRequest>(), null))
                .Throws(new Exception());
        }

        [Test]
        public async Task GetAlbumPublishedYears_ShouldReturnDocumentYearListIfWeReceiveProperRespond()
        {
            // Arrange
            var apiInfoClient = new FacetsClient(RequestHandler.Object, new ApiBaseUri("https://bmm-api.brunstad.org"), Logger.Object);
            RequestHandler.Setup(x => x.GetResolvedResponse<IList<DocumentYear>>(It.IsAny<IRequest>(), null))
                .Returns(Task.FromResult(CreateASampleDocumentYear()));

            // Act
            var result = await apiInfoClient.GetAlbumPublishedYears();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, CreateASampleDocumentYear().Count);
        }

        private IList<DocumentYear> CreateASampleDocumentYear()
        {
            return new List<DocumentYear>(){ new DocumentYear()};
        }
    }
}
