using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IBibleStudyExternalRelationPO : IBasePO
{
    string Title { get; }
    Uri Link { get; }
    bool WillPlayTrack { get; }
    IMvxAsyncCommand ClickedCommand { get; }
}