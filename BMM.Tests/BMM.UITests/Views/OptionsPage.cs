using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IOptionsPage
    {
        Func<AppQuery, AppQuery> RemoveFromPlaylist { get; }
        Func<AppQuery, AppQuery> AddToPlaylist { get; }
        Func<AppQuery, AppQuery> AddToQueue { get; }
        Func<AppQuery, AppQuery> GoToAlbum { get; }
        Func<AppQuery, AppQuery> GoToContributors { get; }
        Func<AppQuery, AppQuery> MoreInformation { get; }
        Func<AppQuery, AppQuery> Cancel { get; }
        Func<AppQuery, AppQuery> DeletePlaylist { get; }
        Func<AppQuery, AppQuery> RenamePlaylist { get; }
    }

    public class AndroidOptionsPage : IOptionsPage
    {
        public Func<AppQuery, AppQuery> RemoveFromPlaylist
        {
            get
            {
                return c => c.Marked("Remove from playlist");
            }
        }

        public Func<AppQuery, AppQuery> AddToPlaylist
        {
            get
            {
                return c => c.Marked("Add to playlist");
            }
        }

        public Func<AppQuery, AppQuery> AddToQueue
        {
            get
            {
                return c => c.Marked("Add to queue");
            }
        }

        public Func<AppQuery, AppQuery> GoToAlbum
        {
            get
            {
                return c => c.Marked("Go to album");
            }
        }

        public Func<AppQuery, AppQuery> GoToContributors
        {
            get
            {
                return c => c.Marked("Go to contributors");
            }
        }

        public Func<AppQuery, AppQuery> MoreInformation
        {
            get
            {
                return c => c.Marked("More information");
            }
        }

        public Func<AppQuery, AppQuery> Cancel
        {
            get
            {
                return c => c.Marked("Cancel");
            }
        }

        public Func<AppQuery, AppQuery> DeletePlaylist
        {
            get
            {
                return c => c.Marked("Delete playlist");
            }
        }

        public Func<AppQuery, AppQuery> RenamePlaylist
        {
            get
            {
                return c => c.Marked("Rename playlist");
            }
        }
    }

    public class TouchOptionsPage : IOptionsPage
    {
        public Func<AppQuery, AppQuery> RemoveFromPlaylist
        {
            get
            {
                return c => c.Marked("Remove from playlist");
            }
        }

        public Func<AppQuery, AppQuery> AddToPlaylist
        {
            get
            {
                return c => c.Marked("Add to playlist");
            }
        }

        public Func<AppQuery, AppQuery> AddToQueue
        {
            get
            {
                return c => c.Marked("Add to queue");
            }
        }

        public Func<AppQuery, AppQuery> GoToAlbum
        {
            get
            {
                return c => c.Marked("Go to album");
            }
        }

        public Func<AppQuery, AppQuery> GoToContributors
        {
            get
            {
                return c => c.Marked("Go to contributors");
            }
        }

        public Func<AppQuery, AppQuery> MoreInformation
        {
            get
            {
                return c => c.Marked("More information");
            }
        }

        public Func<AppQuery, AppQuery> Cancel
        {
            get
            {
                return c => c.Marked("Cancel");
            }
        }

        public Func<AppQuery, AppQuery> DeletePlaylist
        {
            get
            {
                return c => c.Marked("Delete playlist");
            }
        }

        public Func<AppQuery, AppQuery> RenamePlaylist
        {
            get
            {
                return c => c.Marked("Rename playlist");
            }
        }
    }
}