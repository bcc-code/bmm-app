using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Models;
using Moq;
using NUnit.Framework;

namespace BMM.Api.Test.Unit.Implementation.Clients
{
    [TestFixture]
    public class UsersClientTests : ClientTests
    {
        [SetUp]
        protected override void Initialization()
        {
            base.Initialization();
            RequestHandler.Setup(x => x.GetResolvedResponse<User>(It.IsAny<IRequest>(), null, null))
                .Throws(new ResponseException(new HttpRequestMessage(), new HttpResponseMessage(){StatusCode = HttpStatusCode.Forbidden}));
        }

        [Test]
        public void Login_ShouldThrowResponseExceptionWhenThereIsProblemWithAPI()
        {
            // Arrange
            var userClient = new UsersClient(RequestHandler.Object, new ApiBaseUri("https://bmm-api.brunstad.org"), Logger.Object);

            // Act && Assert
            Assert.ThrowsAsync<ResponseException>(() => userClient.Login("SomeAccessToken"));

        }

        [Test]
        public async Task Login_ShouldReturnNullIfUnauthorizedStatusCodeReceived()
        {
            // Arrange
            RequestHandler.Setup(x => x.GetResolvedResponse<User>(It.IsAny<IRequest>(), null, null))
                .Throws(new UnauthorizedException(new HttpRequestMessage(), new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized}));
            var userClient = new UsersClient(RequestHandler.Object, new ApiBaseUri("https://bmm-api.brunstad.org"), Logger.Object);

            // Act
            var result = await userClient.Login("SomeAccessToken");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task Login_ShouldReturnUserIfAccessTokenMatches()
        {
            // Arrange
            RequestHandler.Setup(x => x.GetResolvedResponse<User>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(new User()));
            var userClient = new UsersClient(RequestHandler.Object, new ApiBaseUri("https://bmm-api.brunstad.org"), Logger.Object);

            // Act
            var result = await userClient.Login("SomeAccessToken");

            // Assert
            Assert.IsNotNull(result);

        }

        [Test]
        public async Task Login_ShouldReturnUserIfUsernameAndPasswordMatches()
        {
            // Arrange
            RequestHandler.Setup(x => x.GetResolvedResponse<User>(It.IsAny<IRequest>(), null, null))
                .Returns(Task.FromResult(new User()));
            var userClient = new UsersClient(RequestHandler.Object, new ApiBaseUri("https://bmm-api.brunstad.org"), Logger.Object);

            // Act
            var result = await userClient.Login("UserName", "Password");

            // Assert
            Assert.IsNotNull(result);

        }
    }
}
