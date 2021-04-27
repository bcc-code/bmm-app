using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IQueuePage : IListPage
    {
        Func<AppQuery, AppQuery> Queue { get; }
        Func<AppQuery, AppQuery> CloseQueue { get; }
    }

    public class AndroidQueuePage : ListPage, IQueuePage
    {

        public Func<AppQuery, AppQuery> Queue
        {
            get
            {
                return c => c.Marked("Queue");
            }
        }
        public Func<AppQuery, AppQuery> CloseQueue
        {
            get
            {
                return c => c.Id("menu_queue_player");
            }
        }
    }

    public class TouchQueuePage : ListPage, IQueuePage
    {
        public Func<AppQuery, AppQuery> Queue
        {
            get
            {
                return c => c.Marked("Queue");
            }
        }
        public Func<AppQuery, AppQuery> CloseQueue
        {
            get
            {
                return c => c.Id("backButton");
            }
        }
    }
}
