using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.ValueConverters;

namespace BMM.Core.Extensions;

public static class TrackExtensions
{
    public static bool HasExternalRelations(this Track track)
    {
        return (bool)track.Relations?.Any(relation => relation.Type == TrackRelationType.External);
    }
    
    public static bool IsBibleStudyProjectTrack(this Track track)
    {
        return track.Tags.Any(c =>
            c == PodcastsConstants.ForbildeTagName || c == PodcastsConstants.RomanPodcastTagName ||
            c == PodcastsConstants.GibraltarPodcastTagName);
    }

    public static bool IsForbildeProjectTrack(this Track track)
    {
        return track.Tags.Any(c => c == PodcastsConstants.ForbildeTagName);
    }

    public static string GetPublishDate(this Track track)
    {
        var converter = new TrackToPublishedDateValueConverter();
        return converter.Convert(track,
                null,
                null,
                System.Globalization.CultureInfo.CurrentCulture)
            .ToString();
    }
}