using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.MyContent.Interfaces;
using BMM.Core.Implementations.Factories.TrackCollections;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.PlaylistPersistence;
using BMM.Core.Implementations.Podcasts;
using BMM.Core.Implementations.TrackCollections;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Other;
using BMM.Core.Models.POs.TrackCollections;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels.MyContent
{
    public class DownloadedContentViewModel : ContentBaseViewModel
    {
        private readonly IPrepareDownloadedContentItemsAction _prepareDownloadedContentItemsAction;

        public bool IsEmpty { get; private set; }

        public DownloadedContentViewModel(
            IStorageManager storageManager,
            ITrackCollectionPOFactory trackCollectionPOFactory,
            IPrepareDownloadedContentItemsAction prepareDownloadedContentItemsAction)
            : base(
                storageManager,
                trackCollectionPOFactory)
        {
            _prepareDownloadedContentItemsAction = prepareDownloadedContentItemsAction;
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            Documents.CollectionChanged += DocumentsOnCollectionChanged;
            PropertyChanged += OnPropertyChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            Documents.CollectionChanged -= DocumentsOnCollectionChanged;
            PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsLoading))
                RaisePropertyChanged(() => IsEmpty);
        }

        private void DocumentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => IsEmpty);
        }

        public override async Task Load()
        {
            await base.Load();
            await RaisePropertyChanged(() => IsEmpty);
        }

        public override async Task<IEnumerable<IDocumentPO>> LoadItems(CachePolicy policy = CachePolicy.UseCacheAndRefreshOutdated)
        {
            var allCollectionsExceptMyTracks = (await base.LoadItems(policy))?.OfType<TrackCollectionPO>();
            var items = await _prepareDownloadedContentItemsAction.ExecuteGuarded(allCollectionsExceptMyTracks);
            
            if (!items.Any())
                IsEmpty = true;
            
            return items;
        }
    }
}