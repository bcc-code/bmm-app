using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface ILibraryArchivePage
    {
        Func<AppQuery, AppQuery> Year(string title);
        Func<AppQuery, AppQuery> ArchiveListView { get; }
    }

    public class AndroidLibraryArchivePage : ILibraryArchivePage
    {
        public Func<AppQuery, AppQuery> Year(string title)
        {
                return c => c.Marked(title);
        }

        public Func<AppQuery, AppQuery> ArchiveListView
        {
            get
            {
                return c => c.Id("DocumentsRecyclerView");
            }
        }
    }

    public class TouchLibraryArchivePage : ILibraryArchivePage
    {
        public Func<AppQuery, AppQuery> Year(string title)
        {
             return c => c.Marked(title);
        }

        public Func<AppQuery, AppQuery> ArchiveListView
        {
            get
            {
                return c => c.Marked("archive_year_table");
            }
        }
    }
}
