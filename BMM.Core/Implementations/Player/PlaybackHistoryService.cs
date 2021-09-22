using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Models.PlaybackHistory;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Player
{
    public class PlaybackHistoryService : IPlaybackHistoryService
    {
        public const int MaxEntries = 100;

        private List<PlaybackHistoryEntry> _allEntries;

        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);
        private readonly ICache _cache;

        public PlaybackHistoryService(ICache cache)
        {
            _cache = cache;
        }

        public async Task AddPlayedTrack(IMediaTrack mediaTrack, long lastPosition, DateTime playedAtUTC)
        {
            try
            {
                await _writeSemaphore.WaitAsync();

                mediaTrack = ((Track)mediaTrack).CopyBySerialization();
                mediaTrack.LastPosition = lastPosition;
                mediaTrack.LastPlayedAtUTC = playedAtUTC;

                await GetAll();

                var lastHistoryEntry = _allEntries
                    .LastOrDefault();

                if (lastHistoryEntry != null && lastHistoryEntry.MediaTrack.Id == mediaTrack.Id)
                {
                    lastHistoryEntry.LastPosition = lastPosition;
                    lastHistoryEntry.MediaTrack = (Track)mediaTrack;
                    await _cache.InsertObject(
                        StorageKeys.PlaybackHistory,
                        _allEntries);

                    return;
                }

                if (_allEntries.Count >= MaxEntries)
                    _allEntries.RemoveAt(0);

                _allEntries.Add(new PlaybackHistoryEntry((Track) mediaTrack, lastPosition, playedAtUTC));

                await _cache.InsertObject(
                    StorageKeys.PlaybackHistory,
                    _allEntries);
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        public async Task<IReadOnlyList<PlaybackHistoryEntry>> GetAll()
        {
            if (_allEntries == null)
                await LoadAll();

            return _allEntries.AsReadOnly();
        }

        private async Task LoadAll()
        {
            try
            {
                _allEntries = await _cache.GetObject<List<PlaybackHistoryEntry>>(StorageKeys.PlaybackHistory);
            }
            catch (KeyNotFoundException)
            {
                _allEntries = new List<PlaybackHistoryEntry>();
            }
            catch (Exception)
            {
                await _cache.Invalidate(StorageKeys.PlaybackHistory);
            }
        }
    }
}