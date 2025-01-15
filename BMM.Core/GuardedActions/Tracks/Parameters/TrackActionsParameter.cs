using BMM.Api.Implementation.Models;

namespace BMM.Core.GuardedActions.Tracks.Parameters;

public class TrackActionsParameter
{
    public TrackActionsParameter(Track track, string playbackOrigin)
    {
        Track = track;
        PlaybackOrigin = playbackOrigin;
    }
    
    public Track Track { get; }
    public string PlaybackOrigin { get; }
}