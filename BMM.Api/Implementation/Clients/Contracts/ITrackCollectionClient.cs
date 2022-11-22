using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface ITrackCollectionClient
    {
        /// <summary>Adds the tracks with the specified ids to the track collection.</summary>
        Task<bool> AddTracksToTrackCollection(int id, IList<int> trackIds);

        Task AddAlbumToTrackCollection(int id, int albumId);

        /// <summary>Deletes the track collection.</summary>
        Task<bool> Delete(int id);

        /// <summary>Gets all track collections.</summary>
        Task<IList<TrackCollection>> GetAll(CachePolicy cachePolicy);

        /// <summary>Gets the track collection with the specified identifier.</summary>
        Task<TrackCollection> GetById(int id, CachePolicy cachePolicy);

        /// <summary>Saves the track collection.</summary>
        Task Save(TrackCollection collection);

        /// <summary>Creates a new track collection.</summary>
        Task<int> Create(TrackCollection collection);

        /// <summary>Sets track collection as private</summary>
        Task<bool> ResetShare(int id);

        /// <summary>Unfollows shared track collection</summary>
        Task<bool> Unfollow(int id);
        
        /// <summary>Returns top songs for authorized user</summary>
        Task<TopSongsCollection> GetTopSongs();
        
        /// <summary>Add top songs playlist to favourites</summary>
        Task AddTopSongsToFavourites();
    }
}