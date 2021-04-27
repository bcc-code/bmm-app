using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IMiniPlayer
    {

        Func<AppQuery, AppQuery> MiniPlayer { get; }

        Func<AppQuery, AppQuery> MiniPlayerTrackTitleElement { get; }

        Func<AppQuery, AppQuery> MiniPlayerTrackSubTitleElement { get; }
    }

    public class AndroidMiniPlayer : IMiniPlayer
    {

        public Func<AppQuery, AppQuery> MiniPlayer
        {
            get
            {
                return c => c.Id("miniplayer_frame");
            }
        }

        public Func<AppQuery, AppQuery> MiniPlayerTrackTitleElement
        {
            get
            {
                return c => MiniPlayer(c).Descendant().Id("text_view_title");
            }
        }

        public Func<AppQuery, AppQuery> MiniPlayerTrackSubTitleElement
        {
            get
            {
                return c => MiniPlayer(c).Descendant().Id("text_view_sub_title");
            }
        }

    }

    public class TouchMiniPlayer : IMiniPlayer
    {
        public Func<AppQuery, AppQuery> MiniPlayer
        {
            get 
            {				
                return c => c.Id("MiniPlayerView");
            }
        }

        public Func<AppQuery, AppQuery> MiniPlayerTrackTitleElement
        {
            get
            { 
                return c => MiniPlayer(c).Descendant().Id("TrackTitleLabel");
            }
          
        }

        public Func<AppQuery, AppQuery> MiniPlayerTrackSubTitleElement
        {
            get { return c => MiniPlayer(c).Descendant().Id("TrackSubtitleLabel"); }
        }
    }
}