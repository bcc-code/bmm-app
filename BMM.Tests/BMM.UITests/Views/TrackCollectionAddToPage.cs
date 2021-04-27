using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface ITrackCollectionAddToPage
    {
        Func<AppQuery, AppQuery> TitleHeader { get; }
    }

    public class AndroidTrackCollectionAddToPage : ITrackCollectionAddToPage
    {
        public Func<AppQuery, AppQuery> TitleHeader
        {
            get
            {
                return c => c.Marked("Add track to playlist");
            }
        }
    }

    public class TouchTrackCollectionAddToPage : ITrackCollectionAddToPage
    {
        public Func<AppQuery, AppQuery> TitleHeader
        {
            get
            {
               return c => c.Id("Add track to playlist");
            }
        }
    }
}