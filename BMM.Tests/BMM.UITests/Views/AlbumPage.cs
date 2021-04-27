using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IAlbumPage:IListPage
    {
        Func<AppQuery, AppQuery> AddToPlaylistOption { get; }
        Func<AppQuery, AppQuery> AddToTrackOption { get; }
        Func<AppQuery, AppQuery> Cover { get; }
        Func<AppQuery, AppQuery> AlbumHeader { get; }
        Func<AppQuery, AppQuery> ShuffleButton { get; }
        Func<AppQuery, AppQuery> ListView { get; }
    }

    public class AndroidAlbumPage : ListPage, IAlbumPage
    {
        public Func<AppQuery, AppQuery> AddToPlaylistOption
        {
            get
            {
                return c => c.Marked("Add album to playlist");
            }
        }

        public Func<AppQuery, AppQuery> AddToTrackOption
        {
            get
            {
                return c => c.Marked("icon topbar options static");
            }
        }

        public  Func<AppQuery, AppQuery> Cover
        {
            get
            {
                return c => c.Id("cover_image");
            }
        }

        public Func<AppQuery, AppQuery> AlbumHeader
        {
            get
            {
                return c => c.Id("collapsing_toolbar");
            }
        }

        public Func<AppQuery, AppQuery> ShuffleButton
        {
            get
            {
                return c => c.Id("shuffle_button");
            }
        }

        public Func<AppQuery, AppQuery> ListView
        {
            get
            {
                return c => c.Id("cover_image");
            }
        }

    }

    public class TouchAlbumPage : ListPage, IAlbumPage
    {
        public Func<AppQuery, AppQuery> AddToPlaylistOption
        {
            get
            {
                return c => c.Marked("Add album to playlist");
            }
        }

        public Func<AppQuery, AppQuery> Cover
        {
            get
            {
                return c => c.Id("album_cover");
            }
        }

        public Func<AppQuery, AppQuery> AlbumHeader
        {
            get
            {
                return c => c.Id("album_header");
            }
        }

        public Func<AppQuery, AppQuery> ShuffleButton
        {
            get
            {
                return c => c.Id("shuffle_button");
            }
        }

        public Func<AppQuery, AppQuery> ListView
        {
            get
            {
                return c => c.Class("UITableView").Index(1);
            }
        }

        public Func<AppQuery, AppQuery> AddToTrackOption
        {
            get
            {
                return c => c.Marked("icon topbar options static");
            }
        }
    }
}