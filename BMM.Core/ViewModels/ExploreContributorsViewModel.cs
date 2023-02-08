using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Contributors;

namespace BMM.Core.ViewModels
{
    public class ExploreContributorsViewModel : DocumentsViewModel
    {
        private readonly IAnalytics _analytics;
        private readonly IContributorClient _contributorClient;

        public ExploreContributorsViewModel(
            IAnalytics analytics,
            IContributorClient contributorClient)
        {
            _analytics = analytics;
            _contributorClient = contributorClient;
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var contributors = await _contributorClient.GetFeaturedContributors(policy);
            return contributors.Select(c => new ContributorPO(OptionCommand, c));
        }

        protected override async Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            if (item.DocumentType == DocumentType.Contributor)
            {
                _analytics.LogEvent("User interacts with Single Contributor view");
            }

            await base.DocumentAction(item, list);
        }
    }
}