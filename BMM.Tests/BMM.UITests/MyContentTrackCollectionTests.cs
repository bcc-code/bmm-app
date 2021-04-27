using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace BMM.UITests
{
    [TestFixture(Platform.Android, Category = Categories.Android)]
    [TestFixture(Platform.iOS, Category = Categories.iOS)]
    public class MyContentTrackCollectionTests
    {
        IBmmApp _bmmApp;
        IApp _app;
        readonly Platform _platform;

        const string AddToPlaylist = "Add album to playlist";
        const string AddToTracks = "Add album to My Tracks";
        const string AddAlbumTest = "AddAlbumTest";
        const string MyContent = "My Content";
        const string MyTracks = "Tracks";
        const string TxtToSearch = "Barnas mp3-favoritter 2016";
        const string AlbumNotFound = "The album wanst found";
        const string RemoveMyTrack = "Remove from My Tracks";
        const string RemovePlaylistTrack = "Remove from playlist";


        public MyContentTrackCollectionTests(Platform platform)
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
        public async Task AddAlbumAndTrackToPlaylist_AssertAddedSuccessfully()
        {
            await _bmmApp.LoginToApp();
            await CleanMyTracks(MyContent);
            await Start_Search();
            _app.Tap(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle);
            _app.Tap(AddToPlaylist);
            _app.Tap(AddAlbumTest);
            _app.Repl();
            await Start_Search();
            _app.Tap(_bmmApp.MusicListPage.RowEntries);

            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle, AlbumNotFound, TimeSpan.FromSeconds(20));
            int nTracks = _app.Query(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle).Length;

            _bmmApp.Menu.OpenMyContent(_app);
            _app.Tap(MyContent);
            _app.Tap(AddAlbumTest);
            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle, AlbumNotFound, TimeSpan.FromSeconds(20));
            int nTracks2 = _app.Query(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle).Length;

            Assert.AreEqual(nTracks, nTracks2, AlbumNotFound);
        }

        [Test]
        public async Task AddAlbumAndTrackToMyTracks_AssertAddedSuccessfully()
        {
            await _bmmApp.LoginToApp();
            await CleanMyTracks(MyTracks);
            await Start_Search();
            _app.Tap(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle);
            _app.Tap(AddToTracks);
            await Task.Delay(250);
            await Start_Search();
            _app.Tap(_bmmApp.MusicListPage.RowEntries);

            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle, AlbumNotFound, TimeSpan.FromSeconds(20));
            int nTracks = _app.Query(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle).Length;

            _bmmApp.Menu.OpenMyContent(_app);
            _app.Tap(MyTracks);
            _app.WaitForElement(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle, AlbumNotFound, TimeSpan.FromSeconds(20));
            int nTracks2 = _app.Query(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle).Length;

            Assert.AreEqual(nTracks, nTracks2, AlbumNotFound);
        }

        private async Task Start_Search()
        {
            _bmmApp.Menu.OpenSearch(_app);
            _app.Tap(_bmmApp.NavigationBar.SearchBar);
            _app.EnterText(TxtToSearch);
            _app.PressEnter();
        }

        private async Task CleanMyTracks(string tracktype)
        {
           _bmmApp.Menu.OpenMyContent(_app);
            if (tracktype == MyContent)
            {
                _app.Tap(MyContent);
                _app.Tap(AddAlbumTest);
            }
            else
            {
                _app.Tap(MyTracks);
            }

            await Task.Delay(2000);

            if (_app.Query(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle).Length > 0)
            {
                while (_app.Query(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle).Length > 0)
                {
                    try
                    {
                        _app.Tap(_bmmApp.MyContentPlaylistsPage.PlaylistRowOptionsFromTitle);
                    }
                    catch
                    {
                        break;
                    }

                    await Task.Delay(250);

                    if (tracktype==MyContent)
                    {
                        _app.Tap(RemovePlaylistTrack);
                    }
                    else
                    {
                        _app.Tap(RemoveMyTrack);
                    }

                    await Task.Delay(2500);
                }
            }
        }
    }
}