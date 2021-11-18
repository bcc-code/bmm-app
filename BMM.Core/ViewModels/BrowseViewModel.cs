using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.UI.Interfaces;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class BrowseViewModel : DocumentsViewModel
    {
        private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;
        private readonly ITranslateDocsAction _translateDocsAction;

        public BrowseViewModel(
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
            ITranslateDocsAction translateDocsAction)
        {
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
            _translateDocsAction = translateDocsAction;
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var browseItems = await Client.Browse.Get(policy);

            var translatedItems = await _translateDocsAction.ExecuteGuarded(browseItems.ToList());
            var carouselAdjustedItems = await _prepareCoversCarouselItemsAction.ExecuteGuarded(translatedItems);

            carouselAdjustedItems
                .OfType<DiscoverSectionHeader>()
                .FirstOrDefault()
                .IfNotNull(header => header.IsSeparatorVisible = false);

            return carouselAdjustedItems;
        }
    }
}