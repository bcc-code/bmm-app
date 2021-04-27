using BMM.Api.Implementation.Models;
using System.Collections.Generic;

namespace BMM.Core.Helpers
{
    public interface ITrackMediaHelper
    {
        TrackMediaFile GetFileByMediaType(IEnumerable<TrackMedia> trackMedia, TrackMediaType mediaType);
    }
}