using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IMusicListPage
    {
        Func<AppQuery, AppQuery> RowEntries { get; }
    }

    public class AndroidMusicListPage : IMusicListPage
    {
        public Func<AppQuery, AppQuery> RowEntries
        {
            get
            {
                return c => c.Marked("text_view_title");
            }
        }
    }

    public class TouchMusicListPage : IMusicListPage
    {
        public Func<AppQuery, AppQuery> RowEntries
        {
            get
            {
                return c => c.Id("icon_category_album.png").Index(0);
            }
        }
    }
}