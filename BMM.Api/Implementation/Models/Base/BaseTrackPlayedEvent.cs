namespace BMM.Api.Implementation.Models.Base;

public abstract class BaseTrackPlayedEvent
{
    public int? PersonId { get; set; }
    public int? TrackId { get; set; }
    public DateTime TimestampStart { get; set; }
    public string Language { get; set; }
    public string PlaybackOrigin { get; set; }
    public long LastPosition { get; set; }
    
    /// <summary>
    /// Value of last used playback speed different than default.
    /// E.g if user changes from 1x to 1.5x and then again to 1x, we set 1.5x.
    /// </summary>
    public decimal AdjustedPlaybackSpeed { get; set; }

    public string OS { get; set; }
}