using System;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;

namespace BMM.Core.Implementations.Media
{
    public class MediaFileUrlSetter
    {
        private readonly IStorageManager _storageManager;
        private readonly ITrackMediaHelper _trackMediaHelper;

        public MediaFileUrlSetter(IStorageManager storageManager, ITrackMediaHelper trackMediaHelper)
        {
            _storageManager = storageManager;
            _trackMediaHelper = trackMediaHelper;
        }

        public void SetLocalPathIfDownloaded(IMediaTrack mediaFile)
        {
            if(!(mediaFile is Track track))
            {
                throw new ArgumentException("Can only set url of tracks");
            }

            var trackMediaFile = _trackMediaHelper.GetFileByMediaType(track.Media, track.MediaType);

            if (trackMediaFile != null && _storageManager.SelectedStorage.IsDownloaded(track))
            {
                mediaFile.LocalPath = _storageManager.SelectedStorage.GetUrlByFile(trackMediaFile);
            }
            else
            {
                mediaFile.LocalPath = null;
            }
        }
    }
}