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
using BMM.Core.Implementations.Storage;
using BMM.Core.Models.PlaybackHistory;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Player
{
    public class PlaybackHistoryService : IPlaybackHistoryService
    {
        public const int MaxEntries = 100;

        private List<PlaybackHistoryEntry> _allEntries;

        private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);

        public async Task AddPlayedTrack(IMediaTrack mediaTrack, long lastPosition, DateTime playedAt)
        {
            try
            {
                await _writeSemaphore.WaitAsync();

                mediaTrack = ((Track)mediaTrack).CopyBySerialization();
                mediaTrack.LastPosition = lastPosition;
                mediaTrack.LastPlayedAt = playedAt;

                await GetAll();

                var lastHistoryEntry = _allEntries
                    .LastOrDefault();

                if (lastHistoryEntry != null && lastHistoryEntry.MediaTrack.Id == mediaTrack.Id)
                {
                    lastHistoryEntry.LastPosition = lastPosition;
                    lastHistoryEntry.MediaTrack = (Track)mediaTrack;
                    AppSettings.PlaybackHistory = _allEntries;
                    return;
                }

                if (_allEntries.Count >= MaxEntries)
                    _allEntries.RemoveAt(0);

                _allEntries.Add(new PlaybackHistoryEntry((Track) mediaTrack, lastPosition, playedAt));
                AppSettings.PlaybackHistory = _allEntries;
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
                _allEntries = AppSettings.PlaybackHistory.ToList();
            }
            catch (KeyNotFoundException)
            {
                _allEntries = new List<PlaybackHistoryEntry>();
            }
            catch (Exception)
            {
                AppSettings.PlaybackHistory = null;
            }
        }
    }
}