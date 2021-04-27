using System;
using Xamarin.UITest.Queries;
namespace BMM.UITests.Views
{
    public interface IPodcastPage: IListPage
    {
        Func<AppQuery, AppQuery> Title { get; }
        Func<AppQuery, AppQuery> Getnotified { get; }
        Func<AppQuery, AppQuery> Subsubtitle { get; }
        Func<AppQuery, AppQuery> Follow { get; }
        Func<AppQuery, AppQuery> NumberTrack { get; }
        Func<AppQuery, AppQuery> CoverImage { get; }
        Func<AppQuery, AppQuery> Title_track { get; }
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
                return c => c.Id("text_view_title");
            }
        }

        public Func<AppQuery, AppQuery> Getnotified
        {
            get
            {
                return c => c.Id("text_view_subtitle");
            }
        }

        public Func<AppQuery, AppQuery> Subsubtitle
        {
            get
            {
                return c => c.Id("text_view_sub_subtitle");
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
                return c => c.Id("cover_image");
            }
        }

        public Func<AppQuery, AppQuery> Title_track
        {
            get
            {
                return c => c.Marked("text_view_title");
            }
        }

        public Func<AppQuery, AppQuery> DownloadedImage
        {
            get
            {
                return c => c.Marked("imageView1");
            }
        }

        public Func<AppQuery, AppQuery> Following
        {
            get
            {
                return c => c.Marked("follow_button");
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
                return c => c.Id("FollowTitleLabel");
            }
        }

        public Func<AppQuery, AppQuery> Getnotified
        {
            get
            {
                return c => c.Marked("FollowTitleLabel");
            }
        }

        public Func<AppQuery, AppQuery> Subsubtitle
        {
            get
            {
                return c => c.Id("FollowSubtitleLabel");
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

        public Func<AppQuery, AppQuery> Title_track
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
                return c => c.Marked("icon_downloaded.png");
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
