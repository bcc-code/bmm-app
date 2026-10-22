using System.Collections.Generic;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Downloading
{
    public class UnavailableTrackRegistry : IUnavailableTrackRegistry
    {
        private readonly object _lock = new();
        private HashSet<int> _unavailableTracks;

        private HashSet<int> UnavailableTracks
        {
            get
            {
                // Read through once and keep it in memory: this is consulted for every track of a
                // collection each time the download state is refreshed.
                _unavailableTracks ??= AppSettings.UnavailableTracks;
                return _unavailableTracks;
            }
        }

        public bool IsUnavailable(int trackId)
        {
            lock (_lock)
            {
                return UnavailableTracks.Contains(trackId);
            }
        }

        public void MarkUnavailable(int trackId)
        {
            lock (_lock)
            {
                if (!UnavailableTracks.Add(trackId))
                    return;

                Save();
            }
        }

        public void MarkAvailable(int trackId)
        {
            lock (_lock)
            {
                if (!UnavailableTracks.Remove(trackId))
                    return;

                Save();
            }
        }

        private void Save() => AppSettings.UnavailableTracks = new HashSet<int>(UnavailableTracks);
    }
}
