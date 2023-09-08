using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels.Base;
using MvvmCross;

namespace BMM.Core.ViewModels
{
    public class QueueViewModel : DocumentsViewModel
    {
        private readonly IMediaQueue _mediaQueue;
        private readonly ITrackPOFactory _trackPOFactory;

        public QueueViewModel(
            IMediaQueue mediaQueue,
            ITrackPOFactory trackPOFactory)
        {
            _mediaQueue = mediaQueue;
            _trackPOFactory = trackPOFactory;
        }

        protected override async Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            await Mvx.IoCProvider.Resolve<IMediaPlayer>().Play(_mediaQueue.Tracks, ((TrackPO)item).Track);
        }

        public async override Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return _mediaQueue
                .Tracks
                .ToList()
                .Select(t => _trackPOFactory.Create(TrackInfoProvider, OptionCommand, (Track)t));
        }
    }
}