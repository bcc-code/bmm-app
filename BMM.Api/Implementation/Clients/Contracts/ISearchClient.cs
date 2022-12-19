using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Enums;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface ISearchClient
    {
        /// <summary>Gets all documents matching the specified term.</summary>
        /// <param name="term">The term.</param>
        /// <param name="searchFilter">Filter to apply on results.</param>
        /// <param name="size">The number of items to get.</param>
        /// <param name="from">The number of items to skip.</param>
        /// <returns>The matching documents.</returns>
        Task<SearchResults> GetAll(string term, SearchFilter searchFilter = SearchFilter.All, int from = 0, int size = ApiConstants.LoadMoreSize);
        
        /// <summary>Gets search suggestions for the specified term.</summary>
        /// <param name="term">The term.</param>
        /// <returns>The suggestions.</returns>
        Task<IList<string>> GetSuggestions(string term);
    }
}