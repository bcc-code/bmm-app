using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface ILibraryPodcastsPage
    {
        Func<AppQuery, AppQuery> BtnArchive { get; }
        Func<AppQuery, AppQuery> BtnChildrens { get; }
        Func<AppQuery, AppQuery> BtnFraKare { get; }
        Func<AppQuery, AppQuery> BtnMp3Ftn { get; }
        Func<AppQuery, AppQuery> BtnPodcasts { get; }
        Func<AppQuery, AppQuery> CoverImages { get; }
    }

    public class AndroidLibraryPodcastsPage : ListPage, ILibraryPodcastsPage
    {
        public Func<AppQuery, AppQuery> BtnArchive
        {
            get
            {
                return c => c.Marked("Archive");
            }
        }

        public Func<AppQuery, AppQuery> BtnChildrens
        {
            get
            {
                return c => c.Marked("Childrens MP3-favorites");
            }
        }

        public Func<AppQuery, AppQuery> BtnFraKare
        {
            get
            {
                return c => c.Marked("From Kåre");
            }
        }

        public Func<AppQuery, AppQuery> BtnMp3Ftn
        {
            get
            {
                return c => c.Marked("MP3-Fountain");
            }
        }

        public Func<AppQuery, AppQuery> BtnPodcasts
        {
            get
            {
                return c => c.Marked("Podcasts");
            }
        }

        public Func<AppQuery, AppQuery> CoverImages
        {
            get
            {
                return c => c.Id("podcast_image");
            }
        }
    }

    public class TouchLibraryPodcastsPage : ListPage, ILibraryPodcastsPage
    {
        public Func<AppQuery, AppQuery> BtnArchive
        {
            get
            {
                return c => c.Marked("Archive");
            }
        }

        public Func<AppQuery, AppQuery> BtnChildrens
        {
            get
            {
                return c => c.Marked("Childrens MP3-favorites");
            }
        }

        public Func<AppQuery, AppQuery> BtnFraKare
        {
            get
            {
                return c => c.Marked("From Kåre");
            }
        }

        public Func<AppQuery, AppQuery> BtnMp3Ftn
        {
            get
            {
                return c => c.Marked("MP3-Fountain");
            }
        }

        public Func<AppQuery, AppQuery> BtnPodcasts
        {
            get
            {
                return c => c.Marked("Podcasts");
            }
        }

        public Func<AppQuery, AppQuery> CoverImages
        {
            get
            {
                return c => c.Id("podcast_cover_image");
            }
        }
    }
}
