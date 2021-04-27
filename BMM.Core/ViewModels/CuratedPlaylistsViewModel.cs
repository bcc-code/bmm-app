using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class CuratedPlaylistsViewModel : DocumentsViewModel
    {
        public override CacheKeys? CacheKey => CacheKeys.PlaylistGetAll;

        public static readonly List<int> FeaturedIds = new List<int> { 31, 32, 33, 35, 36, 37 };

        public CuratedPlaylistsViewModel(IDocumentFilter filter = null) : base(filter)
        { }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var playlists = await Client.Playlist.GetAll(policy);
            return playlists;
        }
    }
}
