using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface ISearchPage
    {

        Func<AppQuery, AppQuery> CancelSearch { get; }
        Func<AppQuery, AppQuery> History { get; }
        Func<AppQuery, AppQuery> DeleteHistory { get; }
        Func<AppQuery, AppQuery> ConfirmHistoryDeletion { get; }
        Func<AppQuery, AppQuery> SearchWelcomeTitle { get; }

    }

    public class AndroidSearchPage : ISearchPage
    {
        public Func<AppQuery, AppQuery> CancelSearch
        {
            get
            {
                return c => c.Id("search_close_btn");
            }
        }

        public Func<AppQuery, AppQuery> History
        {
            get
            {
                return c => c.Id("search_history").Descendant().Id("text_view_title");
            }
        }
        public Func<AppQuery, AppQuery> DeleteHistory
        {
            get
            {
                return c => c.Id("relativeLayout1").Descendant().Id("imagebutton_deletehistory");
            }
        }
        public Func<AppQuery, AppQuery> ConfirmHistoryDeletion
        {
            get
            {
                return c => c.Id("buttonPanel").Descendant().Id("button1");
            }
        }
        public Func<AppQuery, AppQuery> SearchWelcomeTitle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }

    public class TouchSearchPage : ISearchPage
    {
        public Func<AppQuery, AppQuery> CancelSearch
        {
            get
            {
               return c => c.Id("clear_button");
            }
        }
        public Func<AppQuery, AppQuery> History
        {
            get
            {
                return c => c.Id("search_history");
            }
        }
        public Func<AppQuery, AppQuery> DeleteHistory
        {
            get
            {
                return c => c.Id("clear_history_button");
            }
        }
        public Func<AppQuery, AppQuery> ConfirmHistoryDeletion
        {
            get
            {
                return c => c.Marked("Ok");
            }
        }
        public Func<AppQuery, AppQuery> SearchWelcomeTitle
        {
            get
            {
                return c => c.Id("welcome_title_label");
            }
        }
    }
}