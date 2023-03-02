using System;
using System.Threading.Tasks;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Models;
using Moq;
using NUnit.Framework;

namespace BMM.Api.Test.Unit.Implementation.Clients
{
    [TestFixture]
    public class ApiInfoClientTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<ApiInfo>(It.IsAny<IRequest>(), null, null))
                .Throws(new Exception());
        }

        [Test]
        public async Task GetById_ShouldReturnApiInfoIfWeReceiveProperRespond()
        {
            // Arrange
            var name = "TestName";
            RequestHandler.Setup(x => x.GetResolvedResponse<ApiInfo>(It.IsAny<IRequest>(), null, null))
                .ReturnsAsync(new ApiInfo() { Name = name });

            var albumClient = new ApiInfoClient(RequestHandler.Object, MockedUri, Logger.Object);

            // Act
            var result = await albumClient.GetInfo();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Name, name);
        }
    }
}
