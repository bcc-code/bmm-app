using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using Moq;
using NUnit.Framework;

namespace BMM.Api.Test.Unit.Implementation.Clients
{
    [TestFixture]
    public abstract class ClientTests
    {
        protected Mock<IRequestHandler> RequestHandler { get; set; }

        protected ApiBaseUri MockedUri { get; } = new ApiBaseUri("https://bmm-api.brunstad.org");

        protected Mock<ILogger> Logger { get; set; }

        [SetUp]
        protected virtual void Initialization()
        {
            RequestHandler = new Mock<IRequestHandler>();
            Logger = new Mock<ILogger>();
        }
    }
}
