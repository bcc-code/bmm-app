using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class MyContentTests : TestBase
    {
        public MyContentTests(Platform platform) : base(platform)
        {
        }

        [Test]
        public async Task WhenContentDownloaded_DownloadIconOnPlaylistVisible()
        {
            await _bmmApp.LoginToApp();

            _bmmApp.Menu.OpenSettings(_app);
            _app.Tap(_bmmApp.SettingsPage.DownloadViaMobileNetworkSwitch);

            _bmmApp.Menu.OpenMyContent(_app);
            _app.Tap("My Content");

            _app.Tap(_bmmApp.MyContentPlaylistsPage.ItemWithTitle(_bmmApp.MyContentPlaylistsPage.UiTestSamplePlaylist.Name));
            _app.Tap(_bmmApp.MyContentPlaylistPage.DownloadSwitch);

            _app.WaitForElement(_bmmApp.MyContentPlaylistPage.AudioFileDownloadedIcon, "Track download icon not found.", TimeSpan.FromSeconds(30));
        }
    }
}
