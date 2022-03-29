using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IExplorePage : IListPage
    {
        Func<AppQuery, AppQuery> FraaKaareTeaser { get; }
        Func<AppQuery, AppQuery> FraaKaareShowAll { get; }
        Func<AppQuery, AppQuery> TrackList { get; }
    }

    public class AndroidExplorePage : ListPage, IExplorePage
    {
        public Func<AppQuery, AppQuery> FraaKaareTeaser
        {
            get
            {
                return c => c.Marked("From Kåre");
            }
        }

        public Func<AppQuery, AppQuery> FraaKaareShowAll
        {
            get
            {
                return c => c.Text("From Kåre");
            }
        }

        public Func<AppQuery, AppQuery> TrackList
        {
            get
            {
                return c => c.Id("my_recycler_view");
            }
        }
    }

    public class TouchExplorePage : ListPage, IExplorePage
    {
        public Func<AppQuery, AppQuery> FraaKaareTeaser
        {
            get
            {
                return c => c.Marked("From Kåre");
            }
        }

        public Func<AppQuery, AppQuery> FraaKaareShowAll
        {
            get
            {
                return c => c.Text("From Kåre");
            }
        }

        public Func<AppQuery, AppQuery> TrackList
        {
            get
            {
                return c => c.Class("UITableView").Index(1);
            }
        }
    }
}