using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class ExplorePageTests
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;

        public ExplorePageTests(Platform platform)
        {
            _platform = platform;
        }

        [SetUp]
        public void Setup()
        {
            _bmmApp = AppInitializer.StartApp(_platform);
            _app = _bmmApp.App;
        }

        [Test]
        public async Task ExplorePage_AssertTabsVisibleAndPopulated()
        {
            await _bmmApp.LoginToApp();

            TestFraaKaarePresent();
        }

        private void TestFraaKaarePresent()
        {
            _app.WaitForElement(_bmmApp.ExplorePage.FraaKaareTeaser, "Fra Kåre teaser couldn't be loaded");
        }
    }
}
