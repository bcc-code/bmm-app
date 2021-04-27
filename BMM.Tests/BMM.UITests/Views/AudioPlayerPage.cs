using System;
using Xamarin.UITest.Queries;

namespace BMM.UITests.Views
{
    public interface IAudioPlayerPage : IListPage
    {
        Func<AppQuery, AppQuery> TrackLength { get; }
        Func<AppQuery, AppQuery> TimeElapsed { get; }
        Func<AppQuery, AppQuery> OpenQueue { get; }
        Func<AppQuery, AppQuery> SeekBar { get; }
        Func<AppQuery, AppQuery> Cover { get; }
        Func<AppQuery, AppQuery> ClosePlayerButton { get; }
        Func<AppQuery, AppQuery> ShuffleButton { get; }
        Func<AppQuery, AppQuery> OptionsButton { get; }
        Func<AppQuery, AppQuery> InfoButton { get; }
        Func<AppQuery, AppQuery> PlayPauseButton { get; }
        Func<AppQuery, AppQuery> PreviousButton { get; }
        Func<AppQuery, AppQuery> NextButton { get; }
        Func<AppQuery, AppQuery> RepeatButton { get; }
        Func<AppQuery, AppQuery> Player { get; }

        Func<AppQuery, AppQuery> PlayerTrackTitleElement { get; }

        Func<AppQuery, AppQuery> PlayerTrackSubTitleElement { get; }

        Func<AppQuery, AppQuery> PlayerAppBarTitleElement { get; }

        Func<AppQuery, AppQuery> PlayerAppBarSubTitleElement { get; }
    }


    public class  AndroidAudioPlayerPage : ListPage, IAudioPlayerPage
    {
        public Func<AppQuery, AppQuery> TrackLength
        {
            get
            {
                return c => c.Marked("textView2");
            }
        }

        public Func<AppQuery, AppQuery> TimeElapsed
        {
            get
            {
                return c => c.Marked("textview_position");
            }
        }
        public Func<AppQuery, AppQuery> OpenQueue
        {
            get
            {
                return c => c.Marked("player_queue_button");
            }
        }
        public Func<AppQuery, AppQuery> SeekBar
        {
            get
            {
                return c => c.Marked("player_seekbar");
            }
        }
        public Func<AppQuery, AppQuery> Cover
        {
            get
            {
                return c => c.Marked("cover_layout");
            }
        }
        public Func<AppQuery, AppQuery> ClosePlayerButton
        {
            get
            {
                return c => c.Marked("closePlayer_button");
            }
        }
        public Func<AppQuery, AppQuery> ShuffleButton
        {
            get
            {
                return c => c.Marked("btnShuffle");
            }
        }
        public Func<AppQuery, AppQuery> OptionsButton
        {
            get
            {
                return c => c.Marked("btnOptions");
            }
        }
        public Func<AppQuery, AppQuery> InfoButton
        {
            get
            {
                return c => c.Marked("imageView3");
            }
        }

        public Func<AppQuery, AppQuery> PlayPauseButton
        {
            get
            {
                return c => c.Marked("btnPlayPause");
            }
        }

        public Func<AppQuery, AppQuery> PreviousButton
        {
            get
            {
                return c => c.Marked("btnPrevious");
            }
        }

        public Func<AppQuery, AppQuery> NextButton
        {
            get
            {
                return c => c.Marked("btnNext");
            }
        }

        public Func<AppQuery, AppQuery> RepeatButton
        {
            get
            {
                return c => c.Marked("btnRepeat");
            }
        }

        public Func<AppQuery, AppQuery> Player
        {
            get
            {
                return c => c.Id("player_frame");
            }
        }

        public Func<AppQuery, AppQuery> PlayerTrackTitleElement
        {
            get
            {
                return c => c.Id("relativeLayout4").Descendant().Id("textView3");
            }
        }

        public Func<AppQuery, AppQuery> PlayerTrackSubTitleElement
        {
            get
            {
                return c => c.Id("relativeLayout4").Descendant().Id("textView4");
            }
        }
        public Func<AppQuery, AppQuery> PlayerAppBarTitleElement
        {
            get
            {
                return c => Player(c).Descendant().Id("textView3");
            }
        }

        public Func<AppQuery, AppQuery> PlayerAppBarSubTitleElement
        {
            get
            {
                return c => Player(c).Descendant().Id("textView4");
            }
        }
    }

    public class TouchAudioPlayerPage : ListPage, IAudioPlayerPage
    {

        public Func<AppQuery, AppQuery> TrackLength
        {
            get
            {
                return c => c.Marked("track_length");
            }
        }

        public Func<AppQuery, AppQuery> TimeElapsed
        {
            get
            {
                return c => c.Marked("textview_position");
            }
        }
        public Func<AppQuery, AppQuery> OpenQueue
        {
            get
            {
                return c => c.Marked("icon queue static");
            }
        }
        public Func<AppQuery, AppQuery> SeekBar
        {
            get
            {
                return c => c.Marked("player_seekbar");
            }
        }
        public Func<AppQuery, AppQuery> Cover
        {
            get
            {
                return c => c.Id("cover_layout");
            }
        }
        public Func<AppQuery, AppQuery> ClosePlayerButton
        {
            get
            {
                return c => c.Marked("icon_down_static.png");
            }
        }
        public Func<AppQuery, AppQuery> ShuffleButton
        {
            get
            {
                return c => c.Id("btnShuffle");
            }
        }
        public Func<AppQuery, AppQuery> OptionsButton
        {
            get
            {
                return c => c.Id("btn_options_static.png");
            }
        }
        public Func<AppQuery, AppQuery> InfoButton
        {
            get
            {
                return c => c.Marked("imageView3");
            }
        }

        public Func<AppQuery, AppQuery> PlayPauseButton
        {
            get
            {
                return c => c.Marked("btnPlayPause");
            }
        }

        public Func<AppQuery, AppQuery> PreviousButton
        {
            get
            {
                return c => c.Marked("btnPrevious");
            }
        }

        public Func<AppQuery, AppQuery> NextButton
        {
            get
            {
                return c => c.Marked("btnNext");
            }
        }

        public Func<AppQuery, AppQuery> RepeatButton
        {
            get
            {
                return c => c.Marked("btnRepeat");
            }
        }
        public Func<AppQuery, AppQuery> Player
        {
            get
            {
                return c => c.Id("PlayerView");
            }
        }

        public Func<AppQuery, AppQuery> PlayerTrackTitleElement
        {
            get
            {
                return c => Player(c).Descendant().Id("TrackTitleLabel");
            }
        }

        public Func<AppQuery, AppQuery> PlayerTrackSubTitleElement
        {
            get
            {
                return c => Player(c).Descendant().Id("TrackSubtitleLabel");
            }
        }

        public Func<AppQuery, AppQuery> PlayerAppBarTitleElement
        {
            get
            {
                return c => Player(c).Descendant().Id("AppbarTitleLabel");
            }
        }

        public Func<AppQuery, AppQuery> PlayerAppBarSubTitleElement
        {
            get
            {
                return c => Player(c).Descendant().Id("AppbarSubtitleLabel");
            }
        }
    }
}