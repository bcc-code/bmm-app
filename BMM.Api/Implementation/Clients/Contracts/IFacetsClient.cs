using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IFacetsClient
    {
        /// <summary>
        ///     Get a list of years where albums were published in.
        /// </summary>
        /// <returns>A list of years.</returns>
        Task<IList<DocumentYear>> GetAlbumPublishedYears();
    }
}