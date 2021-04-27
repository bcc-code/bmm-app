using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IMoreInformationPage
    {
        Func<AppQuery, AppQuery> ArtistHeader(string title);

        Func<AppQuery, AppQuery> Title { get; }

        Func<AppQuery, AppQuery> Album { get; }

        Func<AppQuery, AppQuery> Artist { get; }

        Func<AppQuery, AppQuery> Duration { get; }

        Func<AppQuery, AppQuery> PublishDate { get; }
    }

    public abstract class MoreInformationPage : IMoreInformationPage
    {
        public abstract Func<AppQuery, AppQuery> ArtistHeader(string title);

        public  Func<AppQuery, AppQuery> Title
        {
            get
            {
                return c => c.Marked("Title").Sibling();
            }
        }

        public Func<AppQuery, AppQuery> Album
        {
            get
            {
                return c => c.Marked("Album").Sibling();
            }
        }

        public  Func<AppQuery, AppQuery> Artist
        {
            get
            {
                return c => c.Marked("Artist").Sibling();
            }
        }

        public Func<AppQuery, AppQuery> Duration
        {
            get
            {
                return c => c.Marked("Duration").Sibling();
            }
        }

        public Func<AppQuery, AppQuery> PublishDate
        {
            get
            {
                return c => c.Marked("Publish date").Sibling();
            }
        }

        public class AndroidMoreInformationPage : MoreInformationPage, IMoreInformationPage
        {

            public override Func<AppQuery, AppQuery> ArtistHeader(string title)
            {
                return c => c.Id("toolbar").Child().Marked(title);
            }
        }

        public class TouchMoreInformationPage : MoreInformationPage, IMoreInformationPage
        {
            public override Func<AppQuery, AppQuery> ArtistHeader(string title)
            {
                return c => c.Marked("Ok");
            }
        }

    }
}