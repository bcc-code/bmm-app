using System;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.Security.Oidc.Interfaces;
using Moq;
using MvvmCross.Plugin.Messenger;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.Implementations.Security
{
    [TestFixture]
    public class AccessTokenProviderTests
    {
        private AccessTokenProvider _provider;
        private Mock<IOidcCredentialsStorage> _mockStorage;
        private Mock<IOidcAuthService> _mockAuthService;
        private Mock<IJwtTokenReader> _mockJwtTokenReader;

        [SetUp]
        public void Setup()
        {
            _mockStorage = new Mock<IOidcCredentialsStorage>();
            _mockAuthService = new Mock<IOidcAuthService>();
            _mockJwtTokenReader = new Mock<IJwtTokenReader>();
            _provider = new AccessTokenProvider(
                _mockStorage.Object,
                _mockAuthService.Object,
                Mock.Of<IMvxMessenger>(),
                Mock.Of<ILogger>(),
                _mockJwtTokenReader.Object);
        }

        [Test]
        public async Task ShouldRefresh_WhenExpirationDate_IsOlderThanNow()
        {
            var timeInPast = DateTime.Now.AddHours(-2);
            _mockJwtTokenReader.Setup(x => x.GetExpirationTime(It.IsAny<string>())).Returns(timeInPast);

            var _ = await _provider.GetAccessToken();
            const string failMessage = "The refresh should be called when the access token is expired";
            _mockAuthService.Verify(x => x.RefreshAccessTokenWithRetry(), Times.Once, failMessage);
        }

        [Test]
        public async Task Should_Not_Refresh_WhenExpirationDate_IsNotAboutToExpire()
        {
            var timeInFuture = DateTime.UtcNow.AddHours(AccessTokenProvider.TimeToRefreshTokenBeforeExpirationInHours + 1);
            _mockJwtTokenReader.Setup(x => x.GetExpirationTime(It.IsAny<string>())).Returns(timeInFuture);

            var _ = await _provider.GetAccessToken();
            const string failMessage = "The refresh should not be called when the access token is not expired";
            _mockAuthService.Verify(x => x.RefreshAccessTokenWithRetry(), Times.Never, failMessage);
        }

        [Test]
        public async Task UnexpectedException_WillReturnExpiredAccessToken()
        {
            const string invalidToken = "asfkdsafsdlkfjdsklfjslkj";

            _mockJwtTokenReader.Setup(x => x.GetExpirationTime(It.IsAny<string>())).Returns(DateTime.Now.AddHours(-2));
            _mockStorage.Setup(x => x.GetAccessToken()).ReturnsAsync(invalidToken);
            _mockAuthService.Setup(x => x.RefreshAccessTokenWithRetry()).Throws<OperationCanceledException>();

            await _provider.Initialize();
            var token = await _provider.GetAccessToken();

            // will lead to a 401
            Assert.AreEqual(invalidToken, token);
        }

        [Test]
        public void Throws_OnInternetProblems()
        {
            var timeInPast = DateTime.Now.AddHours(-2);
            _mockJwtTokenReader.Setup(x => x.GetExpirationTime(It.IsAny<string>())).Returns(timeInPast);
            
            _mockAuthService.Setup(x => x.RefreshAccessTokenWithRetry()).Throws(new InternetProblemsException(new Exception("test")));
            Assert.ThrowsAsync<InternetProblemsException>(async () => await _provider.GetAccessToken());
        }
        
        [Test]
        public async Task Returns_WhitespaceOrNull_When_Storage_HasNullValue()
        {
            _mockJwtTokenReader.Setup(x => x.GetExpirationTime(It.IsAny<string>())).Returns(DateTime.Now.AddHours(-2));
            _mockStorage.Setup(x => x.GetAccessToken()).ReturnsAsync("");

            var token = await _provider.GetAccessToken();

            Assert.True(string.IsNullOrWhiteSpace(token));
        }
    }
}