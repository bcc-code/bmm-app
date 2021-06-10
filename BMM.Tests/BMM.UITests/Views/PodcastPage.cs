using System;
using Xamarin.UITest.Queries;
namespace BMM.UITests.Views
{
    public interface IPodcastPage: IListPage
    {
        Func<AppQuery, AppQuery> Title { get; }
        Func<AppQuery, AppQuery> Follow { get; }
        Func<AppQuery, AppQuery> NumberTrack { get; }
        Func<AppQuery, AppQuery> CoverImage { get; }
        Func<AppQuery, AppQuery> TrackTitle { get; }
        Func<AppQuery, AppQuery> DownloadedImage { get; }
        Func<AppQuery, AppQuery> Following { get; }
        Func<AppQuery, AppQuery> PlaylistRowOptionsFromTitle { get; }
    }

    public class AndroidPodcastPage : ListPage, IPodcastPage
    {
        public Func<AppQuery, AppQuery> Title
        {
            get
            {
                return c => c.Id("title");
            }
        }

        public Func<AppQuery, AppQuery> Follow
        {
            get
            {
                return c => c.Id("follow_button");
            }
        }

        public Func<AppQuery, AppQuery> NumberTrack
        {
            get
            {
                return c => c.Id("track_number_text");
            }
        }

        public Func<AppQuery, AppQuery> CoverImage
        {
            get
            {
                return c => c.Id("podcast_image");
            }
        }

        public Func<AppQuery, AppQuery> TrackTitle
        {
            get
            {
                return c => c.Marked("trackTitle");
            }
        }

        public Func<AppQuery, AppQuery> DownloadedImage
        {
            get
            {
                return c => c.Marked("track_download_icon");
            }
        }

        public Func<AppQuery, AppQuery> Following
        {
            get
            {
                return c => c.Marked("following_button");
            }
        }

        public Func<AppQuery, AppQuery> PlaylistRowOptionsFromTitle
        {
            get
            {
                return c => c.Id("btn_options_static");
            }
        }
    }

    public class TouchPodcastPage : ListPage, IPodcastPage
    {
        public Func<AppQuery, AppQuery> Title
        {
            get
            {
                return c => c.Id("title");
            }
        }

        public Func<AppQuery, AppQuery> Follow
        {
            get
            {
                return c => c.Id("follow_button");
            }
        }

        public Func<AppQuery, AppQuery> NumberTrack
        {
            get
            {
                return c => c.Id("track_number_text");
            }
        }

        public Func<AppQuery, AppQuery> CoverImage
        {
            get
            {
                return c => c.Id("podcast_cover");
            }
        }

        public Func<AppQuery, AppQuery> TrackTitle
        {
            get
            {
                return c => c.Marked("TTVC_title");
            }
        }

        public Func<AppQuery, AppQuery> DownloadedImage
        {
            get
            {
                return c => c.Marked("icon_download");
            }
        }

        public Func<AppQuery, AppQuery> Following
        {
            get
            {
                return c => c.Marked("following_button");
            }
        }

        public Func<AppQuery, AppQuery> PlaylistRowOptionsFromTitle
        {
            get
            {
                return c => c.Id("btn_options_static.png");
            }
        }
    }
}
