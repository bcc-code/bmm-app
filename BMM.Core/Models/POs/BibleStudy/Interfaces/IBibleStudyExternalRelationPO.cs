using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IBibleStudyExternalRelationPO : IBasePO
{
    string Title { get; }
    string Subtitle { get; }
    Uri Link { get; }
    bool WillPlayTrack { get; }
    bool ShouldShowPlayAnimation { get; set; }
    IMvxAsyncCommand ClickedCommand { get; }
}