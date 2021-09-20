using System;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Models.PlaybackHistory.Interfaces
{
    public interface IPlaybackHistoryEntry
    {
        Track MediaTrack { get; }
        DateTime PlayedAtUTC { get; }
    }
}