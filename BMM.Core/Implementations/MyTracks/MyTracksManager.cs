using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Messages;
using MvvmCross;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.MyTracks
{
    public class MyTracksManager : IMyTracksManager
    {
        public const string MyTracksPlaylistName = "My Tracks";

        private readonly IBlobCache _blobCache;
        private readonly IBMMClient _bmmClient;
        private readonly IExceptionHandler _exceptionHandler;

        private MvxSubscriptionToken _loggedInOnlineToken;

        private int _myTracksId;
        private TrackCollection _myTracks;

        public MyTracksManager(IBlobCache blobCache, IMvxMessenger messenger, IBMMClient bmmClient, IExceptionHandler exceptionHandler)
        {
            _blobCache = blobCache;
            _bmmClient = bmmClient;
            _exceptionHandler = exceptionHandler;

            _loggedInOnlineToken = messenger.Subscribe<LoggedInOnlineMessage>(message => { Task.Run(LoadMyTracksIdOrCreateItIfNecessary); });
        }

        public async Task InitAsync()
        {
            try
            {
                _myTracksId = await _blobCache.GetOrCreateObject(StorageKeys.MyTracksCollectionId, () => new int(), null);
                await LoadMyTracksIdOrCreateItIfNecessary();
                await LoadMyTracks();
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleExceptionWithoutUserMessages(ex);
            }
        }

        private async Task SaveId()
        {
            await _blobCache.InsertObject(StorageKeys.MyTracksCollectionId, _myTracksId, null);
        }

        public async Task LoadMyTracksIdOrCreateItIfNecessary()
        {
            try
            {
                await GetCachedMyTracksIdOrLoadIt();
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleExceptionWithoutUserMessages(ex);
            }
        }

        public async Task<int> GetCachedMyTracksIdOrLoadIt()
        {
            if (_myTracksId == 0)
            {
                var trackCollections = await _bmmClient.TrackCollection.GetAll(CachePolicy.UseCache);

                // Since "My Tracks" collection can be deleted from the website and the id remains in the blob cache,
                // we specifically check if a playlist named "My Tracks" exists or not
                TrackCollection myTracksCollection = trackCollections.FirstOrDefault(tc => tc.Name.Equals(MyTracksPlaylistName));
                if (myTracksCollection != null)
                {
                    _myTracksId = myTracksCollection.Id;
                    await SaveId();
                }
                else
                {
                    await CreateMyTracksCollection();
                }
            }

            return _myTracksId;
        }

        public async Task<bool> AddTrackToMyTracks(int trackId, string language)
        {
            var trackIdList = new List<int> { trackId };
            var success = await _bmmClient.TrackCollection.AddTracksToTrackCollection(_myTracksId, trackIdList);

            await LoadMyTracks();

            return success;
        }

        public async Task AddAlbumToMyTracks(int albumId)
        {
            var myTracks = new TrackCollection { Id = _myTracksId };

            await Mvx.IoCProvider.Resolve<ITrackCollectionManager>().AddToTrackCollection(myTracks, albumId, DocumentType.Album);

            await LoadMyTracks();
        }

        public async Task<bool> CreateMyTracksCollection()
        {
            await InvalidateMyTracksCollection();

            _myTracks = new TrackCollection
            {
                Name = MyTracksPlaylistName,
                Access = new[] {Mvx.IoCProvider.Resolve<IUserStorage>().GetUser().Username},
                Tracks = new List<Track>()
            };

            try
            {
                var collectionId = await _bmmClient.TrackCollection.Create(_myTracks);
                _myTracksId = collectionId;
                await SaveId();
                Mvx.IoCProvider.Resolve<IAnalytics>().LogEvent("The collection \"My Tracks\" didn't exist so we've created it");

                return true;
            }
            catch (Exception ex)
            {
                Mvx.IoCProvider.Resolve<IExceptionHandler>().HandleException(ex);
            }

            return false;
        }

        public async Task InvalidateMyTracksCollection()
        {
            _myTracksId = 0;
            _myTracks = null;
            await _blobCache.InvalidateObject<int>(StorageKeys.MyTracksCollectionId);
        }

        public async Task<TrackCollection> LoadMyTracks()
        {
            // ToDo: Updating the private variable is an unexpected side effect. We should rework that.
            _myTracks = await _bmmClient.TrackCollection.GetById(_myTracksId, CachePolicy.UseCache);

            return _myTracks;
        }

        public bool MyTracksContainsTrack(int trackId, string language)
        {
            if (_myTracks == null)
            {
                return false;
            }

            foreach (var track in _myTracks.Tracks)
            {
                if (track.Id == trackId && track.Language.Equals(language))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
