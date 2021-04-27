using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IConfirmOptionsPage
    {

        Func<AppQuery, AppQuery> Ok { get; }

        Func<AppQuery, AppQuery> Cancel { get; }

    }

    public class AndroidConfirmOptionsPage : IConfirmOptionsPage
    {

        public Func<AppQuery, AppQuery> Ok
        {
            get
            {
                return c => c.Marked("Ok");
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

    public class TouchConfirmOptionsPage : IConfirmOptionsPage
    {
        public Func<AppQuery, AppQuery> Ok
        {
            get
            {
                return c => c.Marked("Ok");
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