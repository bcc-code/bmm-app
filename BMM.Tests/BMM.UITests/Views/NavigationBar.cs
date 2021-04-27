using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface INavigationBar
    {
        Func<AppQuery, AppQuery> MenuButton { get; }
        Func<AppQuery, AppQuery> SearchBar { get; }
        Func<AppQuery, AppQuery> OptionsButton { get; }
        Func<AppQuery, AppQuery> BackButton { get; }
    }

    public class AndroidNavigationBar : INavigationBar
    {
        public Func<AppQuery, AppQuery> MenuButton
        {
            get
            {
                return c => c.Marked("Open navigation drawer");
            }
        }

        public Func<AppQuery, AppQuery> SearchBar
        {
            get
            {
                return c => c.Marked("search_bar");
            }
        }

        public Func<AppQuery, AppQuery> OptionsButton
        {
            get
            {
                return c => c.Marked("More options");
            }
        }
        public Func<AppQuery, AppQuery> BackButton
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }


    public class TouchNavigationBar : INavigationBar
    {
        public Func<AppQuery, AppQuery> MenuButton
        {
            get
            {
                return c => c.Id("menuButton");
            }
        }

        public Func<AppQuery, AppQuery> SearchBar
        {
            get
            {
                return c => c.Id("search_text_field");
            }
        }

        public Func<AppQuery, AppQuery> OptionsButton
        {
            get
            {
                return c => c.Marked("icon topbar options static");
            }
        }

        public Func<AppQuery, AppQuery> BackButton
        {
            get
            {
                return c => c.Marked("backButton");
            }
        }
    }
}