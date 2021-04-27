using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class LibraryArchiveTests
    {
        private IBmmApp _bmmApp;
        private IApp _app;
        private readonly Platform _platform;

        public LibraryArchiveTests(Platform platform)
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
        public async Task OpenLibraryArchive_AssertAllArchiveYearsExist()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenLibrary(_app);
            _app.Tap("Archive");

            _app.WaitForElement(_bmmApp.LibraryArchivePage.Year(DateTime.Now.Year.ToString()));

            if (_platform == Platform.Android)
                _app.ScrollDown(q => q.Id("collapsing_toolbar"), ScrollStrategy.Gesture);

            _app.ScrollDownTo(_bmmApp.LibraryArchivePage.Year("2017"), _bmmApp.LibraryArchivePage.ArchiveListView, ScrollStrategy.Gesture);
            _app.WaitForElement(_bmmApp.LibraryArchivePage.Year("2017"));

            _app.ScrollDownTo(_bmmApp.LibraryArchivePage.Year("2010"), _bmmApp.LibraryArchivePage.ArchiveListView, ScrollStrategy.Gesture);
            _app.WaitForElement(_bmmApp.LibraryArchivePage.Year("2010"));
        }
    }
}
