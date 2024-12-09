using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.Core.NewMediaPlayer
{
    public class ShuffleableQueue : IShuffleableQueue
    {
        private readonly IMediaQueue _queue;
        private readonly ILogger _logger;
        private IList<IMediaTrack> _shuffledTracks;

        public bool IsShuffleEnabled { get; private set; }

        public RepeatType RepeatMode { get; private set; }

        public ShuffleableQueue(IMediaQueue queue, ILogger logger)
        {
            _queue = queue;
            _logger = logger;
        }

        public void Replace(IMediaTrack track)
        {
            _queue.Replace(track);
        }

        public async Task<bool> Replace(IEnumerable<IMediaTrack> tracks, IMediaTrack currentTrack)
        {
            var result = await _queue.Replace(tracks, currentTrack);
            if (IsShuffleEnabled)
            {
                _shuffledTracks = GetShuffledList(currentTrack);
            }
            return result;
        }

        public Task<bool> Append(IMediaTrack track)
        {
            if (IsShuffleEnabled)
            {
                _shuffledTracks.Add(track);
            }
            return _queue.Append(track);
        }

        public void Delete(IMediaTrack track) => _queue.Delete(track);

        public Task<bool> PlayNext(IMediaTrack track, IMediaTrack currentPlayedTrack)
        {
            if (IsShuffleEnabled)
            {
                var index = _shuffledTracks.IndexOf(currentPlayedTrack) + 1;
                _shuffledTracks.Insert(index, track);
            }

            return _queue.PlayNext(track, currentPlayedTrack);
        }

        public IList<IMediaTrack> Tracks => IsShuffleEnabled ? _shuffledTracks : _queue.Tracks;

        public bool HasPendingChanges
        {
            get => _queue.HasPendingChanges;
            set => _queue.HasPendingChanges = value;
        }

        public bool IsSameQueue(IList<IMediaTrack> newMediaTracks)
        {
            return _queue.IsSameQueue(newMediaTracks);
        }

        public IMediaTrack GetTrackById(int id)
        {
            return _queue.GetTrackById(id);
        }

        public void SetShuffle(bool isShuffleEnabled, IMediaTrack currentTrack)
        {
            if (isShuffleEnabled)
            {
                _shuffledTracks = GetShuffledList(currentTrack);
            }
            IsShuffleEnabled = isShuffleEnabled;
        }

        private List<IMediaTrack> GetShuffledList(IMediaTrack currentTrack)
        {
            var tracks = new List<IMediaTrack>();
            tracks.AddRange(_queue.Tracks);
            var currentTrackIsInQueue = tracks.Remove(currentTrack);
            if (!currentTrackIsInQueue)
            {
                _logger.Warn("ShuffleableQueue", "The current track is not in the queue. That should not happen.");
            }

            ShuffleList(tracks, new Random());

            // the current track is always supposed to be the first
            return tracks.Prepend(currentTrack).ToList();
        }

        /// <summary>
        /// Uses Fisher-Yates algorithm to shuffle a list
        /// </summary>
        public static void ShuffleList<T>(IList<T> list, Random rng)
        {
            var count = list.Count;
            while (count > 1)
            {
                count--;
                var newPosition = rng.Next(count + 1);
                var item = list[newPosition];
                list[newPosition] = list[count];
                list[count] = item;
            }
        }

        public void SetRepeatType(RepeatType type)
        {
            RepeatMode = type;
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}
