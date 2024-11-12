using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.Transcriptions;

public class TranscriptionHeaderPO : BasePO
{
    public TranscriptionHeaderPO(string title, string subtitle)
    {
        Title = title;
        Subtitle = subtitle;
    }
    
    public string Title { get; }
    public string Subtitle { get; }
}