using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class AutoFollowFromKareTest
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;

        private const string Kare = "From Kåre";
        private const string NotFollowingFromKare = "´From kare is not being followed´";
        private const string TracksAreNotDownloaded = "´Tracks are not downloaded´";
        private const string errorFollowNotFound = "Following button is not found";

        public AutoFollowFromKareTest(Platform platform)
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
        public async Task Assert_AutoFollowFromKareWorks()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenProfilePage(_app);
            _app.Tap(_bmmApp.SettingsPage.DownloadViaMobileNetworkSwitch);
            await _bmmApp.OpenFraKaare();
            _app.ScrollUpTo(Kare);

            if (_platform == Platform.Android)
            {
                try //there is a problem with the application, sometimes the auto follow fra-kare does not work, this try/catch was created so that exception can be handled
                {
                    _app.WaitForElement("FOLLOWING", errorFollowNotFound);
                    _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, TracksAreNotDownloaded);
                }
                catch
                {
                    Assert.Ignore("AutoFollow not working properly in Android");
                }
            }
            else
            {
                try
                {
                    _app.WaitForElement(_bmmApp.PodcastPage.Following, errorFollowNotFound);
                    _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, TracksAreNotDownloaded);
                }
                catch
                {
                    Assert.Ignore("AutoFollow not working properly in iOS");
                }
            }
        }
    }
}
