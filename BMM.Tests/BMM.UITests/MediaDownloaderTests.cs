using NUnit.Framework;
using Xamarin.UITest;
using System;
using System.Threading.Tasks;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class MediaDownloaderTests
    {
        private IBmmApp _bmmApp;
        private IApp _app;
        private readonly Platform _platform;
        private const string Childrens = "Childrens MP3-favorites";
        private const string TitleMyContent = "My Content";
        private const string ErrorTracksNotFound = "Tracks were not found";
        private const string ErrorTrackNotFoundBeforePressingSwitch = "Tracks were not found before click switch button";
        private const string ErrorTrackNotFoundAfterPressingSwitch = "Tracks were not found after click swtich button";
        private const string ErrorTrackNotDownloaded = "Tracks were not downloaded";
        private const string ErrorTrackDownloaded = "Tracks are still downloaded";
        private const string ErrorTrackNotFound = "the track was not found";
        private const string ErrorOptionBtnNotFound = "the option button was not found";
        private const string ErrorTrackNotVisible = "The track is not visible or havent been downloaded";

        public MediaDownloaderTests(Platform platform)
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
        public async Task AfterFollowingPodcast_UnfollowPodcast_AssertTracksNotDownloaded()
        {
             await LoginNavigateToFraKaare();
            _app.Tap(_bmmApp.PodcastPage.Follow);
            _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTrackDownloaded, TimeSpan.FromSeconds(60));
            _app.Tap(_bmmApp.PodcastPage.Following);
            _app.Tap("Ok");
            _app.WaitForNoElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTrackDownloaded, TimeSpan.FromSeconds(60));
            await RemoveAndAddPlaylist(false);
        }

        [Test]
        public async Task AfterFollowingPodcastAndAddingToPlaylist_DownloadAndUndownloadPlaylist_AssertTracksStillDownloaded()
        {
            await LoginNavigateToFraKaare();
            AddToPlaylist();
            FollowPodcast();
            _bmmApp.Menu.OpenMyContent(_app);
            _app.Tap(TitleMyContent);
            _app.Tap("TrackMediaReferenceCounting");
            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle, ErrorTrackNotFound, TimeSpan.FromSeconds(7));
            _app.Tap(_bmmApp.MyContentPlaylistPage.DownloadSwitch);
            _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTrackNotDownloaded, TimeSpan.FromSeconds(60));
            _app.Tap(_bmmApp.MyContentPlaylistPage.DownloadSwitch);
            _app.Tap("Ok");
            _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTrackDownloaded, TimeSpan.FromSeconds(60));
            await RemoveAndAddPlaylist(false);
        }

        [Test]
        public async Task AfterAddingAndFollowing_DownloadThenUnfollow_AssertTracksAreDownloaded()
        {
            await LoginNavigateToFraKaare();
            await Task.Delay(1000);
            AddToPlaylist();
            FollowPodcast();
            _bmmApp.Menu.OpenMyContent(_app);
            _app.Tap(TitleMyContent);
            await Task.Delay(5000);
            _app.Repl();
            if(!((_app.Query(c => c.Marked("TrackMediaReferenceCounting").All().Index(0)).Length >0)))
            {
                AddPlaylist();
                _bmmApp.Menu.OpenMyContent(_app);
                _app.Tap(TitleMyContent);
                _app.Repl();
            }

            _app.WaitForElement(c => c.Marked("TrackMediaReferenceCounting"), "The playlist was not added", TimeSpan.FromSeconds(5));
            _app.Tap("TrackMediaReferenceCounting");
            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle, ErrorTrackNotFoundBeforePressingSwitch, TimeSpan.FromSeconds(15));
            _app.Tap(_bmmApp.MyContentPlaylistPage.DownloadSwitch);
            _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTrackNotFoundAfterPressingSwitch, TimeSpan.FromSeconds(60));
            _bmmApp.Menu.OpenLibrary(_app);
            _app.Tap(Childrens);
            await Task.Delay(3000);
            if (_app.Query(_bmmApp.PodcastPage.Following).ToString().Length > 0)
            {
                _app.Tap(_bmmApp.PodcastPage.Following);
            }

            else
            {
                _app.Tap("FOLLOWING");
            }

            await Task.Delay(3000);
            _app.Tap("Ok");
            _app.Tap("Childrens MP3-favorites");
            _bmmApp.Menu.OpenSearch(_app);
            _bmmApp.Menu.OpenMyContent(_app);
            _app.Tap(TitleMyContent);
            _app.WaitForElement(c => c.Marked("TrackMediaReferenceCounting"), "The playlist was not added", TimeSpan.FromSeconds(5));
            _app.Tap("TrackMediaReferenceCounting");
            _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTracksNotFound, TimeSpan.FromSeconds(60));
            await RemoveAndAddPlaylist(false);
        }

        [Test]
        public async Task  AfterFollowAndAddToPlaylist_DownloadUndownloadAndUnfollow_AssertTracksNotDownloaded()
        {
            await LoginNavigateToFraKaare();
            FollowPodcast();
            AddToPlaylist();
            _bmmApp.Menu.OpenMyContent(_app);
            _app.WaitForElement(TitleMyContent);
            _app.Tap(TitleMyContent);
            _app.Tap("TrackMediaReferenceCounting");
            await Task.Delay(4000);

            if (_app.Query(c => c.Id("image_button_options").All().Index(1))!=null)
            {
                _app.Tap(_bmmApp.MyContentPlaylistPage.DownloadSwitch);
                _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTrackDownloaded, TimeSpan.FromSeconds(60));
            } else
            {
                _app.WaitForNoElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTrackNotVisible, TimeSpan.FromSeconds(10));
            }

            await RemoveAndAddPlaylist(false);
        }

        private async Task LoginNavigateToFraKaare()
        {
            await _bmmApp.LoginToApp();
            _bmmApp.Menu.OpenSettings(_app);
            _app.WaitForElement(_bmmApp.SettingsPage.DownloadViaMobileNetworkSwitch,"Download via netwok switch no loaded", TimeSpan.FromSeconds(10));
            _app.Tap("Let the app download playlists using 3G/4G. This may affect your data plan");
            if (_app.Query(c => c.Marked("checkBox1")).Equals(!true))
            {
                _app.Tap(_bmmApp.SettingsPage.DownloadViaMobileNetworkSwitch);
            }
            await RemoveAndAddPlaylist(true);
            _bmmApp.Menu.OpenLibrary(_app);
            _app.Tap(Childrens);
            _app.ScrollUpTo(Childrens);
        }

        private void FollowPodcast()
        {
            _app.Tap(_bmmApp.PodcastPage.Follow);
            _app.WaitForElement(_bmmApp.PodcastPage.DownloadedImage, ErrorTracksNotFound, TimeSpan.FromSeconds(60));
        }

        private async Task RemoveAndAddPlaylist(bool onlyCreate)
        {
            _bmmApp.Menu.OpenMyContent(_app);
            _app.Tap(TitleMyContent);
            await Task.Delay(5000);

            if (_app.Query(c => c.Marked("TrackMediaReferenceCounting").All().Index(0)).Length > 0 && onlyCreate)
            {
                while (_app.Query(c => c.Marked("TrackMediaReferenceCounting").All().Index(0)).Length > 0)
                {
                    _app.Repl();
                     _app.WaitForElement(c => c.Marked("TrackMediaReferenceCounting"), "The playlist was visible in remove and add playlist", TimeSpan.FromSeconds(5));
                    _app.Tap(c => c.Marked("TrackMediaReferenceCounting").All().Index(0));
                    _app.Repl();
                    await Task.Delay(500);
                    _app.Tap(c => c.Marked("icon topbar options static"));
                    await Task.Delay(500);
                    _app.Tap(c => c.Marked("Delete playlist").All().Index(1));
                    await Task.Delay(500);
                    _app.Tap(c => c.Marked("Ok").All().Index(1));
                }

                AddPlaylist();
            }
            else if (onlyCreate)
            {
                AddPlaylist();
            }

            if (onlyCreate == false)
            {
                _app.Repl();
                await Task.Delay(1000);
                _app.Tap(c => c.Marked("TrackMediaReferenceCounting").All().Index(0));
                _app.Repl();
                await Task.Delay(500);
                _app.Tap(c => c.Marked("icon topbar options static"));
                await Task.Delay(500);
                _app.Tap(c => c.Marked("Delete playlist").All().Index(1));
                await Task.Delay(500);
                _app.Tap(c => c.Marked("Ok").All().Index(1));
            }
        }

        private void AddPlaylist()
        {
            _app.Tap(c => c.Marked("UI Test (Don't change)"));
            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle,"Option button in AddPlaylist isn't working",TimeSpan.FromSeconds(5));
            _app.Tap(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle);
            _app.Tap(c => c.Marked("Add to playlist"));
            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.AddButton, "Add button in AddPlaylist isn't working", TimeSpan.FromSeconds(5));
            _app.Tap(_bmmApp.MyContentPlaylistsPage.AddButton);
            _app.EnterText("TrackMediaReferenceCounting");
            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.CreatePlaylistModal.OkButton, "OK button in AddPlaylist isn't working", TimeSpan.FromSeconds(5));
            _app.Tap(_bmmApp.MyContentPlaylistsPage.CreatePlaylistModal.OkButton);
            _app.WaitForElement(c => c.Marked("TrackMediaReferenceCounting"), "The playlist was not added in the AddPlaylist method", TimeSpan.FromSeconds(5));
            _app.Repl(); //To see the backbutton
            _app.WaitForElement(_bmmApp.Menu.UpButton, "back button was not found", TimeSpan.FromSeconds(10));
            _app.Tap(_bmmApp.Menu.UpButton);
            _app.WaitForElement(_bmmApp.Menu.UpButton, "back button was not found", TimeSpan.FromSeconds(5));
        }

        private void AddToPlaylist()
        {
            _app.WaitForElement(_bmmApp.PodcastPage.PlaylistRowOptionsFromTitle, ErrorTracksNotFound, TimeSpan.FromSeconds(30));
            _app.Tap(_bmmApp.PodcastPage.PlaylistRowOptionsFromTitle);
            _app.Tap("Add to playlist");
            _app.WaitForElement(c => c.Marked("TrackMediaReferenceCounting"), "The playlist was not added in the AddToPlaylist method", TimeSpan.FromSeconds(5));
            _app.Tap("TrackMediaReferenceCounting");
        }
    }
}