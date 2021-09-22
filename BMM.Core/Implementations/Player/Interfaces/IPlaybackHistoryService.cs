using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Models.PlaybackHistory;

namespace BMM.Core.Implementations.Player.Interfaces
{
    public interface IPlaybackHistoryService
    {
        Task AddPlayedTrack(IMediaTrack mediaTrack, long lastPosition, DateTime playedAtUTC);
        Task<IReadOnlyList<PlaybackHistoryEntry>> GetAll();
    }
}