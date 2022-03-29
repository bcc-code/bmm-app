using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IBrowsePage
    {
        Func<AppQuery, AppQuery> PodcastsShowAll { get; }
    }
    
    public class TouchBrowsePage : IBrowsePage
    {
        public Func<AppQuery, AppQuery> PodcastsShowAll => c => c.Text("Podcasts").Sibling(1);
    }

    public class AndroidBrowsePage : IBrowsePage
    {
        public Func<AppQuery, AppQuery> PodcastsShowAll => c => c.Text("Podcasts").Sibling(1);
    }
}