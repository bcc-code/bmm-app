using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Albums.Interfaces;
using BMM.Core.Implementations.Analytics;

namespace BMM.Core.Implementations.Albums;

public class AlbumOfflineTrackProvider : IAlbumOfflineTrackProvider
{
    private readonly IOfflineAlbumStorage _offlineAlbumStorage;
    private readonly IAlbumClient _albumClient;
    private readonly IAnalytics _analytics;
    private readonly ILogger _logger;

    public AlbumOfflineTrackProvider(
        IOfflineAlbumStorage offlineAlbumStorage,
        IAlbumClient albumClient,
        IAnalytics analytics,
        ILogger logger)
    {
        _offlineAlbumStorage = offlineAlbumStorage;
        _albumClient = albumClient;
        _analytics = analytics;
        _logger = logger;
    }
    
    public async Task<IList<Track>> GetTracksSupposedToBeDownloaded()
    {
        var toBeDownloaded = new List<Track>();

        var offlineAlbumsIds = _offlineAlbumStorage
            .GetAlbumIds();

        foreach (int albumId in offlineAlbumsIds)
        {
            try
            {
                var album = await _albumClient.GetById(albumId);
                toBeDownloaded.AddRange(album.Children.OfType<Track>());
            }
            catch (NotFoundException)
            {
                RemoveAlbumAndLogAnalytics(albumId, Event.AlbumNotFound);
            }
            catch (Exception e)
            {
                _logger.Error($"Exception while retrieving Album {albumId} to be downloaded", e.ToString());
            }
        }

        return toBeDownloaded;
    }
    
    private void RemoveAlbumAndLogAnalytics(int albumId, string logEventName)
    {
        _analytics.LogEvent(logEventName,
            new Dictionary<string, object>
            {
                {"Id", albumId}
            });
        
        _offlineAlbumStorage.Delete(albumId);
    }
}