using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.MyContent.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Messages;
using BMM.Core.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels.MyContent
{
    public class MyContentViewModel : ContentBaseViewModel
    {
        private readonly IPrepareMyContentItemsAction _prepareMyContentItemsAction;
        private MvxSubscriptionToken _playlistStateChangedMessageSubscriptionKey;

        public MyContentViewModel(
            IStorageManager storageManager,
            ITrackCollectionPOFactory trackCollectionPOFactory,
            IPrepareMyContentItemsAction prepareMyContentItemsAction)
            : base(
                storageManager,
                trackCollectionPOFactory)
        {
            _prepareMyContentItemsAction = prepareMyContentItemsAction;
        }

        protected override Task Initialization()
        {
            _playlistStateChangedMessageSubscriptionKey =
                Messenger.Subscribe<PlaylistStateChangedMessage>(m => ReloadCommand.ExecuteAsync());
            return base.Initialization();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            Messenger.UnsubscribeSafe<PlaylistStateChangedMessage>(_playlistStateChangedMessageSubscriptionKey);
            base.ViewDestroy(viewFinishing);
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollectionsExceptMyTracks = await base.LoadItems(policy);
            return await _prepareMyContentItemsAction.ExecuteGuarded(allCollectionsExceptMyTracks);
        }
    }
}