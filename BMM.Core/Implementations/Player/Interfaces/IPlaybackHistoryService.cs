using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Models.PlaybackHistory.Interfaces;

namespace BMM.Core.Implementations.Player.Interfaces
{
    public interface IPlaybackHistoryService
    {
        Task AddPlayedTrack(IMediaTrack mediaTrack);
        Task<List<PlaybackHistoryEntry>> GetAll();
    }
}