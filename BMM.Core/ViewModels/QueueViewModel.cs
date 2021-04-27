using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.UI;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels.Base;
using MvvmCross;

namespace BMM.Core.ViewModels
{
    // QueueViewModel is actually not dark on Android. We should change that. But right now IDarkStyleViewModel is only used by iOS
    public class QueueViewModel : DocumentsViewModel, IDarkStyleOnIosViewModel
    {
        private readonly IMediaQueue _mediaQueue;

        public QueueViewModel(IMediaQueue mediaQueue)
        {
            _mediaQueue = mediaQueue;
        }

        protected override async Task DocumentAction(Document item, IList<Track> list)
        {
            await Mvx.IoCProvider.Resolve<IMediaPlayer>().Play(_mediaQueue.Tracks, item as IMediaTrack);
        }

        public async override Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            return _mediaQueue.Tracks.OfType<Document>();
        }
    }
}