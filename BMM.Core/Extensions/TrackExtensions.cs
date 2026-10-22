using BMM.Api.Abstraction;
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
    
    public static bool IsSong(this ITrackModel track)
    {
        return track.Subtype.IsOneOf(TrackSubType.Song, TrackSubType.Singsong);
    }
    
    public static IList<IMediaTrack> ToTracksList(this Track track)
    {
        return new List<IMediaTrack>
        {
            track
        };
    }
    
    public static TimeSpan SumTrackDuration(this IEnumerable<Track> tracks)
    {
        long totalSeconds = tracks.Sum(t => t.Duration / 1000);
        return TimeSpan.FromSeconds(totalSeconds);
    }

    /// <summary>
    /// Total size of the files belonging to these tracks.
    /// </summary>
    /// <remarks>
    /// Both <c>media</c> and <c>files</c> are optional in the API, so a single track without them used to
    /// throw a <see cref="NullReferenceException"/> out of the download button's size check, which the user
    /// saw as "An unknown error occurred" with nothing downloaded.
    /// </remarks>
    public static long SumApproximateDownloadSize(this IEnumerable<Track> tracks)
    {
        if (tracks == null)
            return 0;

        return tracks
            .Where(track => track?.Media != null)
            .SelectMany(track => track.Media)
            .Where(medium => medium?.Files != null)
            .SelectMany(medium => medium.Files)
            .Sum(file => file.Size);
    }

    /// <summary>
    /// The tracks of a collection that the downloader will actually try to fetch. Mirrors the filters in
    /// the offline track providers, so that "is everything downloaded" cannot wait for a file that is
    /// never going to be requested.
    /// </summary>
    public static IEnumerable<Track> WhereDownloadable(this IEnumerable<Track> tracks)
    {
        if (tracks == null)
            return Enumerable.Empty<Track>();

        return tracks.Where(track => track != null
                                     && track.Subtype != TrackSubType.Video
                                     && !string.IsNullOrEmpty(track.Url));
    }
}