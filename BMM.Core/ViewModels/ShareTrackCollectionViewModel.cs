using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Messages;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Interfaces;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class ShareTrackCollectionViewModel
        : DocumentsViewModel,
          IShareTrackCollectionViewModel
    {
        private int _trackCollectionId;
        private int _followersCount;
        private string _trackCollectionName;
        private string _shareLink;
        private TrackCollection _trackCollection;

        public ShareTrackCollectionViewModel(IShareLink shareLink)
        {
            ShareCommand = new ExceptionHandlingCommand(async () => await shareLink.PerformRequestFor(_shareLink));
            MakePrivateCommand = new ExceptionHandlingCommand(async () =>
            {
                await Client.TrackCollection.ResetShare(_trackCollectionId);
                _messenger.Publish(new PlaylistStateChangedMessage(this, TrackCollection.Id));
                await ReloadCommand.ExecuteAsync();
            });
        }

        public string TrackCollectionName
        {
            get => _trackCollectionName;
            private set => SetProperty(ref _trackCollectionName, value);
        }

        public int FollowersCount
        {
            get => _followersCount;
            private set => SetProperty(ref _followersCount, value);
        }

        public TrackCollection TrackCollection
        {
            get => _trackCollection;
            private set => SetProperty(ref _trackCollection, value);
        }

        public IMvxAsyncCommand ShareCommand { get; }
        public IMvxAsyncCommand MakePrivateCommand { get; }

        public void Prepare(ITrackCollectionParameter parameter)
        {
            _trackCollectionId = parameter.TrackCollectionId.Value;
        }

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var trackCollection = await Client.TrackCollection.GetById(_trackCollectionId, policy);
            TrackCollection = trackCollection;
            _shareLink = trackCollection.ShareLink;
            TrackCollectionName = trackCollection.Name;
            FollowersCount = trackCollection.FollowerCount;
            return trackCollection.Tracks;
        }
    }
}