using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Factories;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class BrowseViewModel : DocumentsViewModel
    {
        private readonly IDocumentsPOFactory _documentsPOFactory;
        private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;

        public BrowseViewModel(
            IDocumentsPOFactory documentsPOFactory,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction)
        {
            _documentsPOFactory = documentsPOFactory;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var browseItems = await Client.Browse.Get(policy);

            var carouselAdjustedItems = await _prepareCoversCarouselItemsAction.ExecuteGuarded(browseItems.ToList());

            var presentationItems = _documentsPOFactory.Create(
                carouselAdjustedItems,
                DocumentSelectedCommand,
                OptionCommand,
                TrackInfoProvider).ToList();
            
            presentationItems
                .OfType<DiscoverSectionHeaderPO>()
                .FirstOrDefault()
                .IfNotNull(header => header.IsSeparatorVisible = false);
            
            foreach (var discoverSectionHeader in presentationItems.OfType<DiscoverSectionHeaderPO>())
                discoverSectionHeader.Origin = PlaybackOriginString();
                
            presentationItems.Add(new SimpleMarginPO());
            return presentationItems;
        }
    }
}