using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages.MediaPlayer;

public class CurrentTrackWillChangeMessage : MvxMessage
{
    public CurrentTrackWillChangeMessage(object sender, double currentPosition, decimal playbackRate) : base(sender)
    {
        CurrentPosition = currentPosition;
        PlaybackRate = playbackRate;
    }
    
    public double CurrentPosition { get; }
    public decimal PlaybackRate { get; }
}