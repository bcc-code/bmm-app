using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Factories;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class BrowseViewModel : DocumentsViewModel
    {
        private readonly IDocumentsPOFactory _documentsPOFactory;
        private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;
        private readonly ITranslateDocsAction _translateDocsAction;

        public BrowseViewModel(
            IDocumentsPOFactory documentsPOFactory,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
            ITranslateDocsAction translateDocsAction)
        {
            _documentsPOFactory = documentsPOFactory;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
            _translateDocsAction = translateDocsAction;
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var browseItems = await Client.Browse.Get(policy);

            var translatedItems = await _translateDocsAction.ExecuteGuarded(browseItems.ToList());
            var carouselAdjustedItems = await _prepareCoversCarouselItemsAction.ExecuteGuarded(translatedItems);

            carouselAdjustedItems
                .OfType<DiscoverSectionHeader>()
                .FirstOrDefault()
                .IfNotNull(header => header.IsSeparatorVisible = false);

            var presentationItems = _documentsPOFactory.Create(
                carouselAdjustedItems,
                DocumentSelectedCommand,
                OptionCommand,
                TrackInfoProvider).ToList();
            presentationItems.Add(new SimpleMarginPO());
            return presentationItems;
        }
    }
}