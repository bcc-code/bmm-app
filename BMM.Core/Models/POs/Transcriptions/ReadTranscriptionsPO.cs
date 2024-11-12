using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Transcriptions;

public class ReadTranscriptionsPO : BasePO
{
    private bool _isHighlighted;

    public ReadTranscriptionsPO(
        Transcription transcription,
        Action<Transcription> itemClickedAction)
    {
        Transcription = transcription;
        ItemClickedCommand = new MvxCommand(() => itemClickedAction?.Invoke(transcription));
    }
    
    public Transcription Transcription { get; }

    public bool IsHighlighted
    {
        get => _isHighlighted;
        set => SetProperty(ref _isHighlighted, value);
    }

    public IMvxCommand ItemClickedCommand { get; set; }
}