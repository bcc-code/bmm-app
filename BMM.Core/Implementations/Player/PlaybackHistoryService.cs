using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Persistence.Interfaces;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Models.PlaybackHistory;
using BMM.Core.Models.PlaybackHistory.Interfaces;

namespace BMM.Core.Implementations.Player
{
    public class PlaybackHistoryService : IPlaybackHistoryService
    {
        public const int MaxEntries = 100;
        private readonly IBlobCacheWrapper _blobCacheWrapper;

        public PlaybackHistoryService(
            IBlobCacheWrapper blobCacheWrapper)
        {
            _blobCacheWrapper = blobCacheWrapper;
        }

        public async void AddPlayedTrack(IMediaTrack mediaTrack)
        {
            var playbackHistory = await GetPlaybackHistory();

            var lastTrack = playbackHistory
                .LastOrDefault()
                ?.MediaTrack;

            if (lastTrack != null && lastTrack.Id == mediaTrack.Id)
                return;

            if (playbackHistory.Count >= MaxEntries)
                playbackHistory.RemoveAt(0);

            playbackHistory.Add(new PlaybackHistoryEntry((Track) mediaTrack, DateTime.UtcNow));

            await _blobCacheWrapper.InsertObject(
                StorageKeys.PlaybackHistory,
                playbackHistory);
        }

        public async Task<IEnumerable<IPlaybackHistoryGroup>> GetAll()
        {
            var playbackHistory = await GetPlaybackHistory();

            if (!playbackHistory.Any())
                return Enumerable.Empty<IPlaybackHistoryGroup>();

            var result = playbackHistory
                .OrderByDescending(x => x.PlayedAtUTC)
                .GroupBy(l => new
                {
                    l.PlayedAtUTC.Year,
                    l.PlayedAtUTC.Month,
                    l.PlayedAtUTC.Day
                })
                .Where(g => g.Any())
                .Select(g => new PlaybackHistoryGroup(g.ToList(), g.First().PlayedAtUTC))
                .ToList();

            return result;
        }

        private async Task<List<PlaybackHistoryEntry>> GetPlaybackHistory()
        {
            List<PlaybackHistoryEntry> playbackHistoryEntries;

            try
            {
                playbackHistoryEntries = await _blobCacheWrapper.GetObject<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory);
            }
            catch (KeyNotFoundException)
            {
                playbackHistoryEntries = new List<PlaybackHistoryEntry>();
            }

            return playbackHistoryEntries;
        }
    }
}