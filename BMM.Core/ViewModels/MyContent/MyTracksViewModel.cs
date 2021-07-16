using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DocumentFilters;
using BMM.Core.Implementations.Downloading.DownloadQueue;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Messages;
using BMM.Core.ViewModels.Base;
using BMM.Core.ViewModels.Parameters.Interface;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels.MyContent
{
    public class MyTracksViewModel : DownloadViewModel, IMvxViewModel<ITrackCollectionParameter>
    {
        private TrackCollection _myCollection;

        public override string Title => MyCollection.Name;

        public override string Image => null;

        public override bool ShowPlaylistIcon => true;

        public TrackCollection MyCollection
        {
            get => _myCollection;
            protected set
            {
                SetProperty(ref _myCollection, value);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => PlaylistAuthor);
                RaisePropertyChanged(() => CanEdit);
                RaisePropertyChanged(() => FollowersCount);
            }
        }

        public string PlaylistAuthor => MyCollection.AuthorName;
        public bool CanEdit => MyCollection.CanEdit;
        public int FollowersCount => MyCollection.FollowerCount;

        public bool IsEmpty => MyCollection.Tracks?.Count == 0 && !IsLoading && IsInitialized;

        private readonly ITrackCollectionManager _trackCollectionManager;
        private string _playlistAuthor;

        public MyTracksViewModel(
            IStorageManager storageManager,
            IDownloadedTracksOnlyFilter documentFilter,
            ITrackCollectionManager trackCollectionManager,
            IDownloadQueue downloadQueue,
            IConnection connection,
            INetworkSettings networkSettings)
            : base(storageManager, documentFilter, downloadQueue, connection, networkSettings)
        {
            _trackCollectionManager = trackCollectionManager;

            _messenger.Subscribe<DownloadCanceledMessage>(async message =>
                {
                    if (IsDownloading && IsOfflineAvailable)
                    {
                        await _trackCollectionManager.RemoveDownloadedTrackCollection(MyCollection);

                        IsOfflineAvailable = !IsOfflineAvailable;
                        await RaisePropertyChanged(() => Documents);
                    }
                },
                MvxReference.Strong);

            PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(IsOfflineAvailable):
                        RaisePropertyChanged(() => IsDownloading);
                        break;
                    case nameof(IsInitialized):
                        RaisePropertyChanged(() => IsEmpty);
                        break;
                    case nameof(IsLoading):
                        RaisePropertyChanged(() => IsEmpty);
                        break;
                    case nameof(MyCollection):
                        RaisePropertyChanged(() => IsEmpty);
                        RaisePropertyChanged(() => DownloadingText);
                        break;
                    case nameof(DownloadingFiles):
                        RaisePropertyChanged(() => IsDownloading);
                        RaisePropertyChanged(() => DownloadStatus);
                        RaisePropertyChanged(() => DownloadingText);

                        break;
                    case nameof(TextSource):
                        RaisePropertyChanged(() => DownloadingText);
                        break;
                }
            };
        }

        public virtual void Prepare(ITrackCollectionParameter trackCollection)
        {
            MyCollection = new TrackCollection
            {
                Id = trackCollection.TrackCollectionId,
                Name = trackCollection.Name
            };

            IsOfflineAvailable = Mvx.IoCProvider.Resolve<IOfflineTrackCollectionStorage>().IsOfflineAvailable(MyCollection);
        }

        protected override async Task DownloadAction()
        {
            await _trackCollectionManager.DownloadTrackCollection(MyCollection);

            await RaisePropertyChanged(() => Documents);
        }

        protected override async Task DeleteAction()
        {
            await _trackCollectionManager.RemoveDownloadedTrackCollection(MyCollection);
        }

        protected override Task<long> CalculateApproximateDownloadSize()
        {
            return Task.FromResult(MyCollection.Tracks.Sum(x => x.Media.Sum(t => t.Files.Sum(s => s.Size))));
        }

        public override CacheKeys? CacheKey => CacheKeys.TrackCollectionGetById;

        public override async Task<IEnumerable<Document>> LoadItems(CachePolicy cachePolicy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            TrackCollection searchCollection = await Client.TrackCollection.GetById(MyCollection.Id, cachePolicy);

            //Unable to load the data. Therefore we don't change anything and swallow the request.
            //ToDo: find a better user experience than just swallowing requests
            if (searchCollection != null)
            {
                MyCollection = searchCollection;
            }

            return MyCollection.Tracks;
        }
    }
}