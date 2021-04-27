using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android, Ignore = true)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class SearchTests
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;
        private string search = "conference";

        public SearchTests(Platform platform)
        {
            _platform = platform;
        }

        [SetUp]
        public void Setup()
        {
            _bmmApp = AppInitializer.StartApp(_platform);
            _app = _bmmApp.App;
        }

        private async Task OpenSearch()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenSearch(_app);
        }

        private void SearchAndCancelSearch()
        {
            _app.Tap(_bmmApp.NavigationBar.SearchBar);
            _app.EnterText(search);
            _app.PressEnter();
            _app.Tap(_bmmApp.NavigationBar.SearchBar);
            _app.Tap(_bmmApp.SearchPage.CancelSearch);
        }

        [Test]
        public async Task ItemSearch_AssertSearchIsVisibleInHistory()
        {
            string searchHistorySavedError = "The search was not sucessfully saved in the history.";

            await OpenSearch();
            SearchAndCancelSearch();

            string actualHistory = _app.Query(_bmmApp.SearchPage.History)[0].Text;
            Assert.AreEqual(search, actualHistory, searchHistorySavedError);
        }

        [Test]
        public async Task ItemSearch_AssertHistoryIsDeletedSuccessfully()
        {
            string searchHistoryDeletionError = "The search history was not sucessfully deleted.";
            await OpenSearch();
            SearchAndCancelSearch();

            _app.Tap(_bmmApp.SearchPage.DeleteHistory);
            _app.Tap(_bmmApp.SearchPage.ConfirmHistoryDeletion);

            _app.WaitForNoElement(_bmmApp.SearchPage.History, searchHistoryDeletionError, TimeSpan.FromSeconds(3));
        }
    }
}
