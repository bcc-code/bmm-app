using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Downloading;

/// <summary>
/// The tracks a provider believes should be downloaded, together with whether that list could be
/// determined in full. An incomplete list is safe to download from, but must never be used to decide
/// which already downloaded files to delete: a source that momentarily failed to load looks exactly
/// like a source the user unsubscribed from.
/// </summary>
public class OfflineTracksResult
{
    public static OfflineTracksResult Complete(IList<Track> tracks) => new(tracks, true);

    public static OfflineTracksResult Incomplete(IList<Track> tracks) => new(tracks, false);

    private OfflineTracksResult(IList<Track> tracks, bool isComplete)
    {
        Tracks = tracks;
        IsComplete = isComplete;
    }

    /// <summary>
    /// The tracks that were found. A subset of the real list when <see cref="IsComplete"/> is false.
    /// </summary>
    public IList<Track> Tracks { get; }

    /// <summary>
    /// False when at least one source could not be read, meaning tracks may be missing from <see cref="Tracks"/>.
    /// </summary>
    public bool IsComplete { get; }
}
