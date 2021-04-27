using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface ISearchClient
    {
        /// <summary>Gets all documents matching the specified term.</summary>
        /// <param name="term">The term.</param>
        /// <param name="size">The number of items to get.</param>
        /// <param name="from">The number of items to skip.</param>
        /// <param name="resourceTypes">The resource types to filter by.</param>
        /// <param name="contentTypes">The content types to filter by.</param>
        /// <param name="datetimeFrom">The minimum publish date.</param>
        /// <param name="datetimeTo">The maximum publish date.</param>
        /// <param name="tags">The tags to filter by.</param>
        /// <param name="unpublished">Whether to show or hide unpublished documents, or show only unpublished documents.</param>
        /// <returns>The matching documents.</returns>
        Task<IList<Document>> GetAll(string term, int size = ApiConstants.LoadMoreSize, int from = 0, IEnumerable<string> resourceTypes = null,
            IEnumerable<string> contentTypes = null, DateTime? datetimeFrom = null, DateTime? datetimeTo = null, IEnumerable<string> tags = null,
            IEnumerable<string> excludeTags = null, UnpublishedEnum? unpublished = null);

        /// <summary>Gets search suggestions for the specified term.</summary>
        /// <param name="term">The term.</param>
        /// <returns>The suggestions.</returns>
        Task<IList<string>> GetSuggestions(string term);
    }
}