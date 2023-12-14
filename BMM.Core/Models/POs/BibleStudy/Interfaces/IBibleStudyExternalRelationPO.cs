using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.BibleStudy.Interfaces;

public interface IBibleStudyExternalRelationPO : IBasePO, ITrackHolderPO
{
    string Title { get; }
    string Subtitle { get; }
    bool WillPlayTrack { get; }
    IMvxAsyncCommand ClickedCommand { get; }
    bool IsCurrentlyPlaying { get; }
    bool HasListened { get; }
}