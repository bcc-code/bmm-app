using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IMyContentPlaylistsPage : IListPage
    {
        Func<AppQuery, AppQuery> PlaylistListItems { get; }
        Func<AppQuery, AppQuery> AddButton { get; }
        Func<AppQuery, AppQuery> PlaylistRowOptionsFromTitle { get; }
        Func<AppQuery, AppQuery> PlaylistDownloadedIcon { get; }
        ICreatePlaylistModal CreatePlaylistModal { get; }
        IPlaylistOptionsModal PlaylistOptionsModal { get; }
        IUiTestSamplePlaylist UiTestSamplePlaylist { get; }
    }

    public interface IUiTestSamplePlaylist
    {
        string Name { get; }
        string SecondTrackTitle { get; }
        string SecondTrackAlbum { get; }
        string SecondTrackDuration { get; }
        string SecondTrackPublishDate { get; }
    }

    public interface ICreatePlaylistModal
    {
        Func<AppQuery, AppQuery> PlaylistNameTextBox { get; }
        Func<AppQuery, AppQuery> OkButton { get; }
        Func<AppQuery, AppQuery> CancelButton { get; }
    }

    public interface IPlaylistOptionsModal
    {
        Func<AppQuery, AppQuery> DeleteButton { get; }
        Func<AppQuery, AppQuery> RenameButton { get; }
        Func<AppQuery, AppQuery> CancelButton { get; }
        IConfirmPlaylistDeleteModal ConfirmPlaylistDeleteModal { get; }
    }

    public interface IConfirmPlaylistDeleteModal
    {
        Func<AppQuery, AppQuery> OkButton { get; }
        Func<AppQuery, AppQuery> Cancel { get; }
    }

    public class AndroidMyContentPlaylistsPage : ListPage, IMyContentPlaylistsPage
    {
        public Func<AppQuery, AppQuery> PlaylistListItems
        {
            get
            {
                return c => c.Marked("trackcollections_addto").Marked("text_view_title");
            }
        }

        public Func<AppQuery, AppQuery> AddButton
        {
            get
            {
                return c => c.Marked("fab");
            }
        }

        public Func<AppQuery, AppQuery> PlaylistRowOptionsFromTitle
        {
            get
            {
                return c => c.Id("btn_options_static");
            }
        }

        public Func<AppQuery, AppQuery> PlaylistDownloadedIcon
        {
            get
            {
                return c => c.Id("linearLayout1").Child().Id("imageView1");
            }
        }

        public ICreatePlaylistModal CreatePlaylistModal => new AndroidCreatePlaylistModal();

        public IPlaylistOptionsModal PlaylistOptionsModal => new AndroidPlaylistOptionsModal();

        public IUiTestSamplePlaylist UiTestSamplePlaylist => new SamplePlaylist();
    }

    public class AndroidPlaylistOptionsModal : IPlaylistOptionsModal
    {
        public Func<AppQuery, AppQuery> DeleteButton
        {
            get
            {
                return c => c.Marked("Delete playlist");
            }
        }

        public Func<AppQuery, AppQuery> RenameButton
        {
            get
            {
                return c => c.Marked("Rename playlist");
            }
        }

        public Func<AppQuery, AppQuery> CancelButton
        {
            get
            {
                return c => c.Marked("Cancel");
            }
        }

        public IConfirmPlaylistDeleteModal ConfirmPlaylistDeleteModal => new AndroidConfirmPlaylistDeleteModal();
    }


    public class AndroidConfirmPlaylistDeleteModal : IConfirmPlaylistDeleteModal
    {
        public Func<AppQuery, AppQuery> OkButton
        {
            get
            {
                return c => c.Marked("Ok");
            }
        }

        public Func<AppQuery, AppQuery> Cancel
        {
            get
            {
                return c => c.Marked("Cancel");
            }
        }
    }

    public class AndroidCreatePlaylistModal : ICreatePlaylistModal
    {
        public Func<AppQuery, AppQuery> PlaylistNameTextBox
        {
            get
            {
                return c => c.Marked("custom");
            }
        }

        public Func<AppQuery, AppQuery> OkButton
        {
            get
            {
                return c => c.Marked("button1");
            }
        }

        public Func<AppQuery, AppQuery> CancelButton
        {
            get
            {
                return c => c.Marked("button2");
            }
        }
    }

    public class TouchMyContentPlaylistsPage : ListPage, IMyContentPlaylistsPage
    {
        public Func<AppQuery, AppQuery> PlaylistListItems
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Func<AppQuery, AppQuery> AddButton
        {
            get
            {
                return c => c.Marked("icon add static");
            }
        }

        public Func<AppQuery, AppQuery> PlaylistRowOptionsFromTitle
        {
            get
            {
                return c => c.Id("btn_options_static");
            }
        }

        public Func<AppQuery, AppQuery> PlaylistDownloadedIcon
        {
            get
            {
                return c => c.Id("icon_downloaded.png");
            }
        }

        public ICreatePlaylistModal CreatePlaylistModal => new TouchCreatePlaylistModal();

        public class TouchCreatePlaylistModal : ICreatePlaylistModal
        {
            public Func<AppQuery, AppQuery> PlaylistNameTextBox
            {
                get
                {
                    return c => c.Marked("Enter a playlist name");
                }
            }

            public Func<AppQuery, AppQuery> OkButton
            {
                get
                {
                    return c => c.Marked("Ok");
                }
            }

            public Func<AppQuery, AppQuery> CancelButton
            {
                get
                {
                    return c => c.Marked("Cancel");
                }
            }
        }

        public IPlaylistOptionsModal PlaylistOptionsModal
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IUiTestSamplePlaylist UiTestSamplePlaylist => new SamplePlaylist();
    }

    public class SamplePlaylist : IUiTestSamplePlaylist
    {
        public string Name => "UI Test (Don't change)";

        public string SecondTrackTitle => "Arild Tombre";

        public string SecondTrackAlbum => "New Year Conference 2017, Meeting Sat 30. Dec 11:00 AM";

        public string SecondTrackDuration => "09:46";

        public string SecondTrackPublishDate => "12/30/2017";
    }
}