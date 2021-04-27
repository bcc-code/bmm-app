using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class SettingsTests
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;
        private const string PodcastTitleFraKaareGerman = "Von Kåre";
        private const string PodcastTitleFraKaareEnglish = "From Kåre";
        private const string ErrorSongsAreEqual = "The Songs are equal";
        private const string ErrorSongsAreNotEqual = "The Songs are not equal";
        private const string English = "English (English)";
        private const string Deutsch = "Deutsch (German)";
        private const string ErrorNoContentLanguage = "my content language is no visible";

        public SettingsTests(Platform platform)
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
        public async Task ChangeLanguage_AssertLanguageChanged()
        {
            const string languageChangeError = "Language change was not successful as 'Einstellungen' was not found in the menu.";

            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenProfilePage(_app);

            ScrollToAndOpen(_bmmApp.SettingsPage.AppLanguage);
            _app.Tap("Deutsch (German)");
            _app.Back();

            _app.WaitForElement(_bmmApp.Menu.GermanProfileMenuItem, languageChangeError);
            _app.ScrollDownTo(q => q.Marked("Appsprache"), _bmmApp.SettingsPage.SettingsList, ScrollStrategy.Gesture);
        }

        [Test]
        // ToDo: This test fails sometimes
        public async Task AssertContentLanguageChanged()
        {
            await _bmmApp.LoginToApp();

            AddLanguageToMyContentLanguage(Deutsch);

            RemoveTwoTopContentLanguages(_platform);

            OpenLibraryAndPodcast(PodcastTitleFraKaareGerman);
            var songName1 = _app.Query(_bmmApp.PodcastPage.Title_track)[0].Text;
            _app.Back();

            AddLanguageToMyContentLanguage(English);

            RemoveContentLanguageOnTopOfList(_platform);

            OpenLibraryAndPodcast(PodcastTitleFraKaareEnglish);
            var songName2 = _app.Query(_bmmApp.PodcastPage.Title_track)[0].Text;

            Assert.AreNotEqual(songName1, songName2, ErrorSongsAreEqual);

            _app.Back();

            AddLanguageToMyContentLanguage(Deutsch);

            RemoveContentLanguageOnTopOfList(_platform);

            OpenLibraryAndPodcast(PodcastTitleFraKaareGerman);
            var songName1SecondCheck = _app.Query(_bmmApp.PodcastPage.Title_track)[0].Text;
            Assert.AreEqual(songName1, songName1SecondCheck, ErrorSongsAreNotEqual);
        }

        private void AddLanguageToMyContentLanguage(string language)
        {
            _bmmApp.Menu.OpenProfilePage(_app);
            ScrollToAndOpen(_bmmApp.SettingsPage.ContentLanguage);

            if (_platform == Platform.iOS)
            {
                _app.Tap(_bmmApp.ContentLanguagePage.EditBtn);
            }
            _app.Tap(_bmmApp.ContentLanguagePage.AddBtn);
            _app.Tap(language);
        }

        private void ScrollToAndOpen(Func<AppQuery, AppQuery> listItem)
        {
            if (_platform == Platform.Android)
                _app.ScrollDown(q => q.Id("collapsing_toolbar"), ScrollStrategy.Gesture);

            _app.ScrollDownTo(listItem,
                _bmmApp.SettingsPage.SettingsList,
                _platform == Platform.Android ? ScrollStrategy.Gesture : ScrollStrategy.Auto);
            _app.Tap(listItem);
        }

        private void OpenLibraryAndPodcast(string podcastTitle)
        {
            _bmmApp.Menu.OpenLibrary(_app);
            _app.Tap(podcastTitle);
            _app.WaitForElement(_bmmApp.PodcastPage.Title_track, "Timed out waiting for element..."); // Wait until first track appears meaning that the list has loaded
        }

        private void RemoveContentLanguageOnTopOfList(Platform platform)
        {
            if (platform == Platform.iOS)
            {
                _app.Tap(_bmmApp.ContentLanguagePage.DoneBtn);
                IosDelete();
                _app.Back();
            }

            else
            {
                _app.Tap(_bmmApp.ContentLanguagePage.RemoveBtn);
            }
        }

        private void RemoveTwoTopContentLanguages(Platform platform)
        {
            if (platform == Platform.iOS)
            {
                _app.Tap(_bmmApp.ContentLanguagePage.DoneBtn);
                IosDelete();
                IosDelete();
                _app.Back();
            }

            else
            {
                _app.Tap(_bmmApp.ContentLanguagePage.RemoveBtn);
                _app.Tap(_bmmApp.ContentLanguagePage.RemoveBtn);
            }
        }

        private void IosDelete()
        {
            _app.Tap(_bmmApp.ContentLanguagePage.EditBtn);
            _app.Tap(_bmmApp.ContentLanguagePage.RemoveBtn);
            _app.Tap("Delete");
            _app.Tap(_bmmApp.ContentLanguagePage.DoneBtn);
        }
    }
}
