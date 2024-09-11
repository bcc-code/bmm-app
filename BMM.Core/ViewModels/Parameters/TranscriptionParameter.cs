using BMM.Api.Implementation.Models;

namespace BMM.Core.ViewModels.Parameters;

public class TranscriptionParameter
{
    public TranscriptionParameter(int trackId, TrackSubType type)
    {
        TrackId = trackId;
        TrackType = type;
    }

    public int TrackId { get; }
    public TrackSubType TrackType { get; set; }
}