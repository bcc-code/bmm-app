using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages;

public class TrackLikedChangedMessage : MvxMessage
{
    public TrackLikedChangedMessage(object sender, bool isLiked, int trackId) : base(sender)
    {
        IsLiked = isLiked;
        TrackId = trackId;
    }
    
    public bool IsLiked { get; }
    public int TrackId { get; }
}