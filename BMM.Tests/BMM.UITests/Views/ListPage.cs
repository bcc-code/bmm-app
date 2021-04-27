using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IListPage
    {
        Func<AppQuery, AppQuery> ItemWithTitle(string title);
        Func<AppQuery, AppQuery> OptionsButton(int index);
    }

    public class ListPage : IListPage
    {
        public Func<AppQuery, AppQuery> ItemWithTitle(string title)
        {
            return c => c.Marked(title);
        }

        public Func<AppQuery, AppQuery> OptionsButton (int index)
        {
            return c => c.Id("image_button_options").Index(index);
        }
    }
}