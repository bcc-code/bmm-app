using System.Linq;
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
            const int maxScrollTries = 20;
            int currentScrollTries = 0;

            _app.WaitForElement(_bmmApp.ExplorePage.ContinueCollectionCarousel);
            
            while (!_app.Query(_bmmApp.ExplorePage.FraaKaareTeaser).Any() && currentScrollTries < maxScrollTries)
            {
                currentScrollTries++;
                _app.SwipeRightToLeft(_bmmApp.ExplorePage.ContinueCollectionCarousel, 0.3, 100);
            }
            
            _app.WaitForElement(_bmmApp.ExplorePage.FraaKaareTeaser, "Fra Kåre teaser couldn't be loaded");
        }
    }
}
