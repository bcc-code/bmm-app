    using System;
    using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IMyContentPage
    {
        Func<AppQuery, AppQuery> MyContentButton { get; }
        Func<AppQuery, AppQuery> OfflineContentButton { get; }
        Func<AppQuery, AppQuery> Playlists { get; }
        Func<AppQuery, AppQuery> PlaylistSubTitle { get; }
        Func<AppQuery, AppQuery> Podcasts { get; }
        Func<AppQuery, AppQuery> Tracks { get; }
    }

    public class AndroidMyContentPage : IMyContentPage
    {
        public Func<AppQuery, AppQuery> MyContentButton
        {
            get { return c => c.Marked("My Content"); }
        }

        public Func<AppQuery, AppQuery> OfflineContentButton
        {
            get { return c => c.Marked("Offline content"); }
        }

        public Func<AppQuery, AppQuery> Playlists
        {
            get { return c => c.Marked("Playlists"); }
        }

        public Func<AppQuery, AppQuery> PlaylistSubTitle
        {
            get { return c => c.Marked("text_view_sub_title"); }
        }

        public Func<AppQuery, AppQuery> Podcasts
        {
            get { return c => c.Marked("Podcasts"); }
        }

        public Func<AppQuery, AppQuery> Tracks
        {
            get { return c => c.Marked("Tracks"); }
        }
    }

    public class TouchMyContentPage : IMyContentPage
    {
        public Func<AppQuery, AppQuery> MyContentButton
        {
            get { return c => c.Text("My Content"); }
        }

        public Func<AppQuery, AppQuery> OfflineContentButton
        {
            get { return c => c.Text("Offline content"); }
        }

        public Func<AppQuery, AppQuery> Playlists
        {
            get { return c => c.Marked("TrackTable_Title"); }
        }

        public Func<AppQuery, AppQuery> PlaylistSubTitle
        {
            get { return c => c.Marked("TrackTable_Subtitle"); }
        }

        public Func<AppQuery, AppQuery> Podcasts
        {
            get { return c => c.Marked("Podcasts"); }
        }

        public Func<AppQuery, AppQuery> Tracks
        {
            get { return c => c.Marked("Tracks"); }
        }
    }
}