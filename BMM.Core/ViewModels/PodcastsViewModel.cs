using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Podcasts;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class PodcastsViewModel : DocumentsViewModel
    {
        public override CacheKeys? CacheKey => CacheKeys.PodcastGetAll;

        public PodcastsViewModel(IDocumentFilter filter = null) : base(filter)
        { }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return (await Client.Podcast.GetAll(policy))?.Select(p => new PodcastPO(p));
        }
    }
}