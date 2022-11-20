using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Tracks.Interfaces;
using BMM.Core.Implementations.Factories.YearInReview;
using BMM.Core.Models.POs.Tracks.Interfaces;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Tracks
{
    public class PrepareTopSongsViewModelAction
        : GuardedActionWithResult<IEnumerable<ITrackPO>>,
          IPrepareTopSongsViewModelAction
    {
        private readonly ITrackCollectionClient _trackCollectionClient;
        private readonly ITopSongsPOFactory _topSongsPOFactory;

        public PrepareTopSongsViewModelAction(
            ITrackCollectionClient trackCollectionClient,
            ITopSongsPOFactory topSongsPOFactory)
        {
            _trackCollectionClient = trackCollectionClient;
            _topSongsPOFactory = topSongsPOFactory;
        }

        private ITopSongsCollectionViewModel DataContext => this.GetDataContext();

        protected override async Task<IEnumerable<ITrackPO>> Execute()
        {
            var topSongs = await _trackCollectionClient.GetTopSongs();
            DataContext.Name = topSongs.Name;
            DataContext.PageTitle = topSongs.PageTitle;
            return topSongs
                .Tracks
                .Select(t => _topSongsPOFactory.Create(t, DataContext.OptionCommand));
        }
    }
}