using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.Transcriptions;

public class ReadTranscriptionsPO : BasePO
{
    private bool _isHighlighted;

    public ReadTranscriptionsPO(Transcription transcription)
    {
        Transcription = transcription;
    }
    
    public Transcription Transcription { get; }

    public bool IsHighlighted
    {
        get => _isHighlighted;
        set => SetProperty(ref _isHighlighted, value);
    }
}