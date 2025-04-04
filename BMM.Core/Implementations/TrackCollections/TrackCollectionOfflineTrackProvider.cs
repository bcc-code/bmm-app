﻿using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Analytics;

namespace BMM.Core.Implementations.TrackCollections
{
    public class TrackCollectionOfflineTrackProvider : ITrackCollectionOfflineTrackProvider
    {
        private readonly IBMMClient _client;
        private readonly IOfflineTrackCollectionStorage _trackCollectionStorage;
        private readonly IAnalytics _analytics;

        public TrackCollectionOfflineTrackProvider(IBMMClient client,
            IOfflineTrackCollectionStorage trackCollectionStorage,
            IAnalytics analytics)
        {
            _client = client;
            _trackCollectionStorage = trackCollectionStorage;
            _analytics = analytics;
        }

        public async Task<IList<Track>> GetTracksSupposedToBeDownloaded()
        {
            var allOfflineTracksInTrackCollections = new List<Track>();
            var offlineTrackCollectionIds = _trackCollectionStorage.GetOfflineTrackCollectionIds().ToList();
            foreach (var id in offlineTrackCollectionIds)
            {
                TrackCollection offlineTrackCollection = null;

                try
                {
                    offlineTrackCollection = await GetTrackCollection(id);
                }
                catch (NotFoundException)
                {
                    await RemoveTrackCollectionAndLogAnalytics(id, "Downloaded playlist was deleted from another device");
                }
                catch (ForbiddenException)
                {
                    await RemoveTrackCollectionAndLogAnalytics(id, "Unauthorized to access playlist");
                }

                if (offlineTrackCollection != null)
                {
                    offlineTrackCollection.Tracks = offlineTrackCollection.Tracks.Where(track => track.Subtype != TrackSubType.Video).ToList();
                    allOfflineTracksInTrackCollections.AddRange(offlineTrackCollection.Tracks);
                }
            }

            return allOfflineTracksInTrackCollections;
        }

        private async Task<TrackCollection> GetTrackCollection(int trackCollectionId)
        {
            return await _client.TrackCollection.GetById(trackCollectionId, CachePolicy.UseCacheAndWaitForUpdates);
        }

        private async Task RemoveTrackCollectionAndLogAnalytics(int trackCollectionId, string logEventName)
        {
            _analytics.LogEvent(logEventName,
                new Dictionary<string, object>
                {
                    {"Id", trackCollectionId},
                    {"TrackCollectionId", trackCollectionId}
                });
            await _trackCollectionStorage.Remove(trackCollectionId);
        }
    }
}