using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IContributorClient
    {
        Task<int> Add(Contributor contributor);

        /// <summary>Gets all contributors matching the specified parameters.</summary>
        /// <param name="size">The number of items to get.</param>
        /// <param name="from">The number of items to skip.</param>
        /// <param name="orderBy">The property to order by.</param>
        /// <returns>A list of contributors.</returns>
        Task<IList<Contributor>> GetAll(int size = ApiConstants.LoadMoreSize, int from = 0, string orderBy = null);

        Task<Contributor> GetById(int id);

        /// <summary>Gets the contributors matching the specified term.</summary>
        /// <param name="term">The term.</param>
        /// <param name="size">The number of items to get.</param>
        /// <returns>The matching contributors.</returns>
        Task<IList<Contributor>> GetByTerm(string term, int size = ApiConstants.LoadMoreSize);

        /// <summary>Gets the cover for the specified contributor.</summary>
        /// <param name="contributorId">The contributor identifier.</param>
        /// <returns>The cover stream.</returns>
        Task<Stream> GetCover(int contributorId);

        /// <summary>Gets the tracks for the specified contributor.</summary>
        /// <param name="contributorId">The contributor identifier.</param>
        /// <param name="size">The number of items to get.</param>
        /// <param name="from">The number of items to skip.</param>
        /// <param name="contentTypes">List of types of the contents.</param>
        /// <returns>The tracks.</returns>
        Task<IList<Track>> GetTracks(int contributorId, int size = ApiConstants.LoadMoreSize, int from = 0, IEnumerable<string> contentTypes = null);
    }
}