using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Media;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.Translation;
using BMM.Core.ViewModels.MyContent;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Localization;

namespace BMM.Core.NewMediaPlayer
{
    /// <summary>
    /// Stores the tracks that are currently relevant for playing tracks. Which means the current queue.
    /// </summary>
    public class MediaQueue : IMediaQueue
    {
        private readonly MediaFileUrlSetter _mediaFileUrlSetter;
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public IList<IMediaTrack> Tracks { get; private set; }

        public MediaQueue(
            MediaFileUrlSetter mediaFileUrlSetter,
            IToastDisplayer toastDisplayer,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _mediaFileUrlSetter = mediaFileUrlSetter;
            _toastDisplayer = toastDisplayer;
            _bmmLanguageBinder = bmmLanguageBinder;
            Tracks = new List<IMediaTrack>();
        }

        public void Replace(IMediaTrack track)
        {
            _mediaFileUrlSetter.SetLocalPathIfDownloaded(track);
            Tracks = new List<IMediaTrack> { track };
        }

        public async Task<bool> Replace(IEnumerable<IMediaTrack> tracks, IMediaTrack currentTrack)
        {
            var filteredList = tracks.Where(m => m.MediaType == TrackMediaType.Audio).ToList();

            // Caution: if the track is downloading while we add it here, it effectively means that the track is downloaded twice
            foreach (var mediaTrack in filteredList)
            {
                _mediaFileUrlSetter.SetLocalPathIfDownloaded(mediaTrack);
            }

            // todo this should be handled without checking the UI!
            var tsc = new TaskCompletionSource<bool>();
            await Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(() =>
            {
                tsc.SetResult(Mvx.IoCProvider.Resolve<IViewModelAwareViewPresenter>().IsViewModelInStack<DownloadedContentViewModel>());
            });

            var excludeNotDownloadedTracks = await tsc.Task;

            if (excludeNotDownloadedTracks)
            {
                filteredList = filteredList.Where(m => m.Availability == ResourceAvailability.Local).ToList();

                if (!filteredList.Any() || (currentTrack != null && currentTrack.Availability != ResourceAvailability.Local))
                {
                    await _toastDisplayer.ErrorAsync(_bmmLanguageBinder[Translations.MediaPlayer_ErrorFileNotOffline]);
                    return false;
                }
            }

            Tracks = filteredList;
            return true;
        }

        public async Task<bool> Append(IMediaTrack track)
        {
            if (await FileNotDownloadedButInOfflineViewModel(track))
                return false;

            Tracks.Add(track);
            return true;
        }

        public async Task<bool> PlayNext(IMediaTrack track, IMediaTrack currentPlayedTrack)
        {
            var nextPlayedIndex = Tracks.IndexOf(currentPlayedTrack) + 1;

            if (Tracks.Count < nextPlayedIndex)
                return false;

            if (await FileNotDownloadedButInOfflineViewModel(track))
                return false;

            Tracks.Insert(nextPlayedIndex, track);

            return true;
        }

        // todo this should be handled without checking the UI!
        private async Task<bool> FileNotDownloadedButInOfflineViewModel(IMediaTrack track)
        {
            _mediaFileUrlSetter.SetLocalPathIfDownloaded(track);

            var tsc = new TaskCompletionSource<bool>();
            await Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(() =>
            {
                tsc.SetResult(Mvx.IoCProvider.Resolve<IViewModelAwareViewPresenter>().IsViewModelInStack<DownloadedContentViewModel>());
            });

            var shouldExcludeNotDownloadedTracks = await tsc.Task;
            if (shouldExcludeNotDownloadedTracks)
            {
                if (track.Availability != ResourceAvailability.Local)
                {
                    await _toastDisplayer.WarnAsync(_bmmLanguageBinder[Translations.MediaPlayer_ErrorFileNotOffline]);
                    return true;
                }
            }

            return false;
        }

        public bool IsSameQueue(IList<IMediaTrack> newMediaTracks)
        {
            if (newMediaTracks.Any(t => t.IsLivePlayback))
                return false;

            if (newMediaTracks.Count != Tracks.Count)
                return false;

            var comparer = new MediaTrackComparer();
            var onlyInNew = newMediaTracks.Except(Tracks, comparer);
            var onlyInQueue = Tracks.Except(newMediaTracks, comparer);
            
            bool tracksChanged = onlyInNew.Any() || onlyInQueue.Any();

            if (tracksChanged)
                return false;

            return CheckAllTracksOnTheSamePosition(newMediaTracks);
        }

        private bool CheckAllTracksOnTheSamePosition(IList<IMediaTrack> newMediaTracks)
        {
            bool allTracksOnTheSamePosition = true;

            for (int i = 0; i < Tracks.Count; i++)
            {
                var queueTrack = Tracks[i];
                var newTrack = newMediaTracks[i];

                if (queueTrack.Equals(newTrack))
                    continue;

                allTracksOnTheSamePosition = false;
                break;
            }

            return allTracksOnTheSamePosition;
        }

        public IMediaTrack GetTrackById(int id)
        {
            return Tracks.FirstOrDefault(t => t.Id == id);
        }

        public void Clear()
        {
            Tracks = new List<IMediaTrack>();
        }
    }

    public class MediaTrackComparer : IEqualityComparer<IMediaTrack>
    {
        public bool Equals(IMediaTrack x, IMediaTrack y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(IMediaTrack obj)
        {
            return obj.GetHashCode();
        }
    }
}
