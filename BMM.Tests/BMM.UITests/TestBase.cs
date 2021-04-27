using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    public class TestBase
    {
        protected readonly Platform _platform;

        protected IBmmApp _bmmApp;

        protected IApp _app;

        public TestBase(Platform platform)
        {
            _platform = platform;
        }

        [SetUp]
        public void Setup()
        {
            _bmmApp = AppInitializer.StartApp(_platform);
            _app = _bmmApp.App;
        }

        protected void IgnoreTestOnIos(string message)
        {
            if (_platform == Platform.iOS)
                Assert.Ignore(message);
        }
    }
}
