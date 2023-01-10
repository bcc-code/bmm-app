using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class AlbumViewTests
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;

        public AlbumViewTests(Platform platform)
        {
            _platform = platform;
        }

        [SetUp]
        public void Setup()
        {
            _bmmApp = AppInitializer.StartApp(_platform);
            _app = _bmmApp.App;
        }

        private async Task NavigateToAlbumView()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenSearch(_app);
            _app.Tap(_bmmApp.NavigationBar.SearchBar);
            _app.EnterText("brunstadfest austria 2016");

            if (_platform == Platform.Android)
                ((AndroidApp)_app).PressUserAction();
            else
                _app.PressEnter();

            _app.Tap(_bmmApp.AlbumPage.ItemWithTitle("Brunstadfest Austria 2016"));
        }

        [Test]
        public async Task NavigateToAlbumView_AssertUiElementsAreCorrect()
        {
            string missingError = "is missing or incorrect.";
            string pageOptionsError = "The options for the album " + missingError;
            string shuffleButtonError = "The shuffle button is missing.";
            string albumCoverError = "The album cover image " + missingError;
            string titleHeaderError = "The title header " + missingError;

            await NavigateToAlbumView();

            _app.WaitForElement(_bmmApp.AlbumPage.ItemWithTitle("Medley"), "the page was not loaded");

            _app.Tap(_bmmApp.NavigationBar.OptionsButton);
            _app.WaitForElement(_bmmApp.AlbumPage.AddToPlaylistOption, pageOptionsError);

            CloseContextOptionsMenu();

            _app.WaitForElement(_bmmApp.AlbumPage.AlbumHeader, titleHeaderError);
            _app.WaitForElement(_bmmApp.AlbumPage.Cover, albumCoverError);

            _app.WaitForElement(_bmmApp.AlbumPage.ShuffleButton, shuffleButtonError);
        }

        private void CloseContextOptionsMenu()
        {
            if (_platform == Platform.Android)
                _app.TapCoordinates(200, 200);
            else
                _app.Tap(q => q.Marked("Cancel"));
        }
    }
}