using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IAlbumClient
    {
        /// <summary>Gets all albums matching the specified parameters.</summary>
        /// <param name="size">The number of items to get.</param>
        /// <param name="from">The number of items to skip.</param>
        /// <param name="contentTypes">The content types to get.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A list of albums.</returns>
        Task<IList<Album>> GetAll(int size = ApiConstants.LoadMoreSize, int from = 0, IEnumerable<TrackSubType> contentTypes = null,
            IEnumerable<string> tags = null, IEnumerable<string> excludeTags = null);

        /// <summary>Gets the album with the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The byalbum.</returns>
        Task<Album> GetById(int id);

        /// <summary>Gets the cover for the specified album.</summary>
        /// <param name="albumId">The album identifier.</param>
        /// <returns>The cover response.</returns>
        Task<Stream> GetCover(int albumId);

        /// <summary>Gets albums published in the specified year.</summary>
        /// <param name="year">The year.</param>
        /// <returns>A list of albums.</returns>
        Task<IList<Album>> GetPublishedByYear(int year);

        /// <summary>Gets albums recorded in the specified year.</summary>
        /// <param name="year">The year.</param>
        /// <returns>A list of albums.</returns>
        Task<IList<Album>> GetRecordedByYear(int year);
    }
}