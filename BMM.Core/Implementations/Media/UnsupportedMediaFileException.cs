using System;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Media
{
    public class UnsupportedMediaFileException: Exception
    {
        public TrackMediaType MediaType { get; }

        public UnsupportedMediaFileException(TrackMediaType mediaType): base("MediaFileType not supported")
        {
            MediaType = mediaType;
        }
    }
}
