using BMM.Api.Abstraction;

namespace BMM.Core.ViewModels.Parameters;

public class TranscriptionParameter
{
    public TranscriptionParameter(ITrackModel track)
    {
        Track = track;
    }

    public ITrackModel Track { get; }
}