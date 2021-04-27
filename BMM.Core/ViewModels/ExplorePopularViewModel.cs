using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using System.Collections.Generic;
using BMM.Api.Abstraction;

namespace BMM.Core.ViewModels
{
    public class ExplorePopularViewModel : LoadMoreDocumentsViewModel
    {
        public ExplorePopularViewModel()
            : base()
        { }

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            return await Client.Statistics.GetGlobalDownloadedMost(DocumentType.Track, size, startIndex);
        }
    }
}