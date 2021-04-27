using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IPodcastClient
    {
        Task<IList<Podcast>> GetAll(CachePolicy cachePolicy);

        Task<Podcast> GetById(int id, CachePolicy cachePolicy);

        /// <summary>
        ///     Gets the cover for the specified album.
        /// </summary>
        /// <param name="podcastId">The podcast identifier.</param>
        /// <returns>The cover response.</returns>
        Task<Stream> GetCover(int podcastId);

        /// <summary>
        ///     Gets the tracks for the specified podcast
        /// </summary>
        /// <param name="size">The number of items to get.</param>
        /// <param name="from">The number of items to skip.</param>
        /// <returns>A list of tracks.</returns>
        Task<IList<Track>> GetTracks(int podcastId, CachePolicy cachePolicy, int size = ApiConstants.LoadMoreSize, int from = 0);

        Task<Track> GetRandomTrack(int podcastId);
    }
}