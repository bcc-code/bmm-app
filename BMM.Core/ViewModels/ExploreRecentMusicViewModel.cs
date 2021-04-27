using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.TrackInformation.Strategies;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class ExploreRecentMusicViewModel : LoadMoreDocumentsViewModel
    {
        public override CacheKeys? CacheKey => CacheKeys.TracksGetAll;

        private readonly IEnumerable<TrackSubType> _trackSubTypes = new List<TrackSubType>
        {
            TrackSubType.Song
        };

        public ExploreRecentMusicViewModel()
        {
            TrackInfoProvider = new TypeKnownTrackInfoProvider();
        }

        public override async Task<IEnumerable<Document>> LoadItems(int startIndex, int size, CachePolicy policy)
        {
            return await Client.Tracks.GetAll(cachePolicy: policy,
                size: size,
                @from: startIndex,
                contentTypes: _trackSubTypes);
        }
    }
}