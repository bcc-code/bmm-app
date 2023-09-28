using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.SuggestEdit.Interfaces;

namespace BMM.Core.Models.POs.SuggestEdit;

public class TranscriptionPO : BasePO, ITranscriptionPO
{
    private readonly Action _editedAction;
    private readonly string _initialText;
    private string _text;

    public TranscriptionPO(
        Transcription transcription,
        Action editedAction)
    {
        _editedAction = editedAction;
        Transcription = transcription;
        _initialText = transcription.Text;
        Text = _initialText;
    }

    public string Text
    {
        get => _text;
        set
        {
            SetProperty(ref _text, value);
            _editedAction?.Invoke();
        }
    }

    public Transcription Transcription { get; }
    public bool HasChanges => _initialText != Text;
}