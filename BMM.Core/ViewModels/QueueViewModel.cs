using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Implementations.Factories.Tracks;
using BMM.Core.Implementations.UI;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tracks;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels
{
    public class QueueViewModel : DocumentsViewModel
    {
        private readonly IMediaQueue _mediaQueue;
        private readonly ITrackPOFactory _trackPOFactory;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IMvxMessenger _mvxMessenger;
        private MvxSubscriptionToken _token;

        public QueueViewModel(
            IMediaQueue mediaQueue,
            ITrackPOFactory trackPOFactory,
            IMediaPlayer mediaPlayer,
            IMvxMessenger mvxMessenger)
        {
            _mediaQueue = mediaQueue;
            _trackPOFactory = trackPOFactory;
            _mediaPlayer = mediaPlayer;
            _mvxMessenger = mvxMessenger;
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            _token = _mvxMessenger.Subscribe<QueueChangedMessage>(QueueChangedAction);
        }
        
        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            _mvxMessenger.UnsubscribeSafe<QueueChangedMessage>(_token);
        }
        
        private async void QueueChangedAction(QueueChangedMessage queueChangedMessage)
        {
            await Refresh();
        }
        
        protected override async Task DocumentAction(IDocumentPO item, IList<Track> list)
        {
            await _mediaPlayer.Play(_mediaQueue.Tracks, ((TrackPO)item).Track);
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