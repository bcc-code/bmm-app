using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IContentLanguagePage
    {
        Func<AppQuery, AppQuery> Language { get; }
        Func<AppQuery, AppQuery> RemoveBtn { get; }
        Func<AppQuery, AppQuery> Handler { get; }
        Func<AppQuery, AppQuery> AddBtn { get; }
        Func<AppQuery, AppQuery> EditBtn { get; }
        Func<AppQuery, AppQuery> DoneBtn { get; }
    }

    public class AndroidContentLanguagePage : IContentLanguagePage
    {
        public Func<AppQuery, AppQuery> Language
        {
            get
            {
                return c => c.Id("textView3");
            }
        }

        public Func<AppQuery, AppQuery> RemoveBtn
        {
            get
            {
                return c => c.Id("delete");
            }
        }

        public Func<AppQuery, AppQuery> Handler
        {
            get
            {
                return c => c.Id("handle");
            }
        }

        public Func<AppQuery, AppQuery> AddBtn
        {
            get
            {
                return c => c.Id("fab");
            }
        }

        public Func<AppQuery, AppQuery> EditBtn
        {
            get
            {
                return c=>c.Marked("textView3");
            }
        }
        
        public Func<AppQuery, AppQuery> DoneBtn
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    public class TouchContentLanguagePage : IContentLanguagePage
    {
        public Func<AppQuery, AppQuery> Language
        {
            get
            {
                return c => c.Id("LangText");
            }
        }

        public Func<AppQuery, AppQuery> RemoveBtn
        {
            get
            {
                return c => c.Class("UITableViewCellEditControl").Index(0);
            }
        }

        public Func<AppQuery, AppQuery> Handler
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Func<AppQuery, AppQuery> AddBtn
        {
            get
            {
                return c => c.Marked("Add");
            }
        }

        public Func<AppQuery, AppQuery> EditBtn
        {
            get
            {
                return c => c.Marked("Edit");
            }
        }

        public Func<AppQuery, AppQuery> DoneBtn
        {
            get
            {
                return c => c.Marked("Done");
            }
        }
    }
}
