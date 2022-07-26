using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Documents.Interfaces;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Security;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class CuratedPlaylistsViewModel : DocumentsViewModel
    {
        private readonly IAppLanguageProvider _appLanguageProvider;
        private readonly IUserStorage _userStorage;
        private readonly ITranslateDocsAction _translateDocsAction;
        private readonly IPrepareCoversCarouselItemsAction _prepareCoversCarouselItemsAction;
        public override CacheKeys? CacheKey => CacheKeys.PlaylistGetAll;

        public CuratedPlaylistsViewModel(
            IAppLanguageProvider appLanguageProvider,
            IUserStorage userStorage,
            ITranslateDocsAction translateDocsAction,
            IPrepareCoversCarouselItemsAction prepareCoversCarouselItemsAction)
        {
            _appLanguageProvider = appLanguageProvider;
            _userStorage = userStorage;
            _translateDocsAction = translateDocsAction;
            _prepareCoversCarouselItemsAction = prepareCoversCarouselItemsAction;
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
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
            
            carouselAdjustedItems.Add(new SimpleMargin());
            return carouselAdjustedItems;
        }
    }
}
