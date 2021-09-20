using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Models.PlaybackHistory.Interfaces;

namespace BMM.Core.Implementations.Player.Interfaces
{
    public interface IPlaybackHistoryService
    {
        void AddPlayedTrack(IMediaTrack mediaTrack);
        Task<IEnumerable<IPlaybackHistoryGroup>> GetAll();
    }
}