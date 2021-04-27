using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IMyContentPodcastFollowedPage
    {
        Func<AppQuery, AppQuery> FollowedPodcast { get; }
        Func<AppQuery, AppQuery> FollowedImage { get; }
        Func<AppQuery, AppQuery> FollowBtn { get; }
    }

    public class AndroidMyContentPodcastFollowedPage : IMyContentPodcastFollowedPage
    {
        public Func<AppQuery, AppQuery> FollowedPodcast
        {
            get
            {
                return c => c.Marked("title");
            }
        }
   
        public Func<AppQuery, AppQuery> FollowedImage
        {
            get
            {
                return c => c.Marked("podcast_image");
            }
        }


        public Func<AppQuery, AppQuery> FollowBtn
        {
            get
            {
                return c => c.Marked("follow_button");
            }
        }
    }

    public class TouchMyContentPodcastFollowedPage : IMyContentPodcastFollowedPage
    {
        public Func<AppQuery, AppQuery> FollowedPodcast
        {
            get
            {
                return c => c.Marked("title");
            }
        }

        public Func<AppQuery, AppQuery> FollowedImage
        {
            get
            {
                return c => c.Marked("podcast_cover_image");
            }
        }

        public Func<AppQuery, AppQuery> FollowBtn
        {
            get
            {
                return c => c.Marked("follow_button");
            }
        }
    }
}