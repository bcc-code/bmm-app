using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using BMM.Api.RequestInterceptor;
using BMM.Core.Implementations.ApiClients;
using BMM.Core.Implementations.Security;
using Moq;
using NUnit.Framework;

namespace BMM.Core.Test.Integration.Client
{
    [TestFixture]
    [Category("Integration")]
    public class BmmClientTests
    {
        private BMMClient _bmmClient;

        [SetUp]
        public void Setup()
        {
            var factory = new TestRequestHandlerFactory();
            var languageManager = new LanguageManager();

            var interceptor = new RequestInterceptorComposite
            {
                RequestInterceptors = new List<IRequestInterceptor>
                {
                    new HeaderRequestInterceptor(new List<IHeaderProvider>
                    {
                        new ContentLanguageHeaderProvider(languageManager),
                        new JsonContentTypeHeaderProvider()
                    }),
                    new TokenRequestInterceptor(), new UserAgentRequestInterceptor()
                }
            };

            _bmmClient = new BMMClient(new ApiBaseUri("https://bmm-api.brunstad.org"),
                interceptor,
                factory,
                new Mock<ILogger>().Object);
        }

        [Test]
        public async Task Track_GetById_ReturnsATrack()
        {
            // Arrange
            // Assert
            var track = await _bmmClient.Tracks.GetById(97607);

            // Assert
            Assert.AreEqual("Den gudfryktige ser evangeliet og lever det", track.Title);
        }

        [Test]
        public void Track_GetById_ThrowsNotFoundException()
        {
            // Arrange
            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(() => _bmmClient.Tracks.GetById(1));
        }

        public class TestRequestHandlerFactory : IRequestHandlerFactory
        {
            public IRequestHandler CreateInstance(IRequestInterceptor requestInterceptor = null)
            {
                return new RequestHandler(new SimpleHttpClient(new HttpClient()), null, requestInterceptor, null, new ResponseDeserializer());
            }

            public IRequestHandler BuildUnauthorizedRequestHandler()
            {
                throw new NotImplementedException();
            }

            public IRequestHandler BuildRequestHandler()
            {
                throw new NotImplementedException();
            }
        }

        public class TokenRequestInterceptor : IRequestInterceptor
        {
            public Task InterceptRequest(IRequest request)
            {
                var user = "ola.normann";
                var token = "e2cf257fa093288c19a5160671910858";

                var authHeader = user + ":" + token;
                var bytes = Encoding.UTF8.GetBytes(authHeader);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(bytes);

                return Task.CompletedTask;
            }
        }

        public class UserAgentRequestInterceptor : IRequestInterceptor
        {
            public Task InterceptRequest(IRequest request)
            {
                request.Headers.Add("User-Agent", "BMM Integration Tests");
                return Task.CompletedTask;
            }
        }

        public class LanguageManager : IContentLanguageManager
        {
            private string[] _contentLanguages = {"nb", "en"};

            public Task<IEnumerable<string>> GetContentLanguages()
            {
                return Task.FromResult(_contentLanguages.AsEnumerable());
            }

            public Task SetContentLanguages(IEnumerable<string> languages)
            {
                _contentLanguages = languages.ToArray();
                return Task.CompletedTask;
            }

            public Task<IEnumerable<string>> GetContentLanguagesIncludingHidden()
            {
                return Task.FromResult(_contentLanguages.AsEnumerable());
            }
        }
    }
}
