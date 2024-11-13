using BMM.Core.Models.POs.Base;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Transcriptions;

public class TranscriptionHeaderPO : BasePO
{
    public TranscriptionHeaderPO(
        string title,
        string subtitle,
        IMvxCommand headerClickedCommand)
    {
        Title = title;
        Subtitle = subtitle;
        HeaderClickedCommand = headerClickedCommand;
    }
    
    public string Title { get; }
    public string Subtitle { get; }
    public IMvxCommand HeaderClickedCommand { get; set; }
}