using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IMyContentPlaylistPage : IListPage
    {
        Func<AppQuery, AppQuery> DownloadSwitch { get; }
        Func<AppQuery, AppQuery> DownloadProgressBar { get; }
        Func<AppQuery, AppQuery> AudioFileDownloadedIcon { get; }
        Func<AppQuery, AppQuery> Delete { get; }
        Func<AppQuery, AppQuery> Rename { get; }
        Func<AppQuery, AppQuery> FollowAdd { get; }
        Func<AppQuery, AppQuery> TextBox { get; }
    }

    public class AndroidMyContentPlaylistPage : ListPage, IMyContentPlaylistPage
    {
        public Func<AppQuery, AppQuery> DownloadSwitch
        {
            get
            {
                return c => c.Switch("download_switch");
            }
        }

        public Func<AppQuery, AppQuery> DownloadProgressBar
        {
            get
            {
                return c => c.Marked("progressbar_downloading");
            }
        }

        public Func<AppQuery, AppQuery> AudioFileDownloadedIcon
        {
            get
            {
                return c => c.Id("linearLayout1").Child().Id("imageView1");
            }
        }

        public Func<AppQuery, AppQuery> Delete
        {
            get
            {
                return c => c.Id("title").Marked("Delete");
            }
        }
        public Func<AppQuery, AppQuery> Rename
        {
            get
            {
                return c => c.Id("title").Marked("Rename");
            }
        }

        public Func<AppQuery, AppQuery> FollowAdd
        {
            get
            {
                return c => c.Id("fab");
            }
        }
        public Func<AppQuery, AppQuery> TextBox
        {
            get
            {
                return c => c.Id("NoResourceEntry-2147483647");
            }
        }
    }

    public class TouchMyContentPlaylistPage : ListPage, IMyContentPlaylistPage
    {
        public Func<AppQuery, AppQuery> DownloadSwitch
        {
            get
            {
                return c => c.Marked("Available offline");
            }
        }

        public Func<AppQuery, AppQuery> DownloadProgressBar
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Func<AppQuery, AppQuery> AudioFileDownloadedIcon
        {
            get
            {
                return c=>c.Marked("icon_downloaded.png");
            }
        }
        public Func<AppQuery, AppQuery> Delete
        {
            get
            {
                return c => c.Marked("Delete playlist");
            }
        }
        public Func<AppQuery, AppQuery> Rename
        {
            get
            {
                return c => c.Marked("Rename playlist");
            }
        }

        public Func<AppQuery, AppQuery> FollowAdd
        {
            get
            {
                return c => c.Id("fab");
            }
        }

        public Func<AppQuery, AppQuery> TextBox
        {
            get
            {
                return c => c.Id("NoResourceEntry - 2147483647");
            }
        }
    }
}