using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IGoToContributorPage
    {
        Func<AppQuery, AppQuery>  Performer { get; }

        Func<AppQuery, AppQuery> Cancel { get; }
    }

    public class AndroidGoToContributorPage : IGoToContributorPage
    {
        public Func<AppQuery, AppQuery> Performer
        {
            get
            {
                return c => c.Id("select_dialog_listview").Descendant().Id("text1");
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

    public class TouchGoToContributorPage : IGoToContributorPage
    {
        public Func<AppQuery, AppQuery> Performer
        {
            get
            {
                return c => Cancel(c);
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
}