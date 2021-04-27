using BMM.Api.Implementation.Models;
using System.Collections.Generic;
using System.Linq;

namespace BMM.Core.Helpers
{
    public abstract class TrackMediaHelper : ITrackMediaHelper
    {
        /// <summary>
        ///   Gets the supported MIME types.
        ///
        ///   Possible known MIME types, returned by the API are:
        ///   "audio/mpeg" - MP3 audio files
        ///   "video/mp4"  - MP4 video files (H.264 + AAC)
        ///   "video/webm" - WebM video files (VP8 + Vorbis)
        /// </summary>
        /// <value>The supported MIME types.</value>
        protected abstract string[] SupportedMimeTypes { get; }

        public TrackMediaFile GetFileByMediaType(IEnumerable<TrackMedia> trackMedia, TrackMediaType mediaType)
        {
            TrackMediaFile currentFile = null;
            if (trackMedia == null)
            {
                return null;
            }
            foreach (TrackMedia media in trackMedia)
            {
                if (media.Type != mediaType)
                {
                    continue;
                }

                foreach (TrackMediaFile file in media.Files)
                {
                    if (SupportedMimeTypes.Contains(file.MimeType))
                    {
                        currentFile = file;
                        break;
                    }
                }

                if (currentFile != null)
                {
                    break;
                }
            }

            return currentFile;
        }
    }
}