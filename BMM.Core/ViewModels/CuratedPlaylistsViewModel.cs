using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Security;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class CuratedPlaylistsViewModel : DocumentsViewModel
    {
        private readonly IUserStorage _userStorage;
        private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;
        private readonly IDocumentsPOFactory _documentsPOFactory;
        public override CacheKeys? CacheKey => CacheKeys.PlaylistGetAll;

        public CuratedPlaylistsViewModel(IUserStorage userStorage,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
            IDocumentsPOFactory documentsPOFactory)
        {
            _userStorage = userStorage;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
            _documentsPOFactory = documentsPOFactory;
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var playlistsDocuments = await Client
                .Playlist
                .GetDocuments(_userStorage.GetUser().Age, policy);

            var carouselAdjustedItems = await _prepareCoversCarouselItemsAction.ExecuteGuarded(playlistsDocuments.Items.ToList());

            var presentationItems = _documentsPOFactory.Create(
                carouselAdjustedItems,
                DocumentSelectedCommand,
                OptionCommand,
                TrackInfoProvider).ToList();
            
            presentationItems
                .OfType<DiscoverSectionHeaderPO>()
                .FirstOrDefault()
                .IfNotNull(header => header.IsSeparatorVisible = false);
            
            presentationItems.Add(new SimpleMarginPO());
            return presentationItems;
        }
    }
}
