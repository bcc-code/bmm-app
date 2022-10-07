using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Factories;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Security;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class CuratedPlaylistsViewModel : DocumentsViewModel
    {
        private readonly IAppLanguageProvider _appLanguageProvider;
        private readonly IUserStorage _userStorage;
        private readonly ITranslateDocsAction _translateDocsAction;
        private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;
        private readonly IDocumentsPOFactory _documentsPOFactory;
        public override CacheKeys? CacheKey => CacheKeys.PlaylistGetAll;

        public CuratedPlaylistsViewModel(
            IAppLanguageProvider appLanguageProvider,
            IUserStorage userStorage,
            ITranslateDocsAction translateDocsAction,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction,
            IDocumentsPOFactory documentsPOFactory)
        {
            _appLanguageProvider = appLanguageProvider;
            _userStorage = userStorage;
            _translateDocsAction = translateDocsAction;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
            _documentsPOFactory = documentsPOFactory;
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var playlistsDocuments = await Client
                .Playlist
                .GetDocuments(_appLanguageProvider.GetAppLanguage(), _userStorage.GetUser().Age, policy);

            var translatedItems = await _translateDocsAction.ExecuteGuarded(playlistsDocuments.Items.ToList());
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
