using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Factories.ContinueListening;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using BMM.Core.Models.POs.Tiles.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tiles
{
    public class TileCollectionPO : DocumentPO, ITrackHolderPO
    {
        public TileCollectionPO(
            ITilePOFactory tilePOFactory,
            IMvxCommand<IDocumentPO> documentSelectedCommand,
            IMvxAsyncCommand<Document> optionClickedCommand,
            TilesCollection tilesCollection) : base(tilesCollection)
        {
            DocumentSelectedCommand = documentSelectedCommand;
            var tilesPOs = tilesCollection
                .TileElements
                .Select(doc => tilePOFactory.Create(optionClickedCommand, doc))
                .ToList();

            foreach (var tilePO in tilesPOs.OfType<MessageTilePO>())
                tilePO.RemoveItemFromObservableCollection = RemoveItemFromObservableCollection;
            
            Tiles.AddRange(tilesPOs);
        }

        private void RemoveItemFromObservableCollection(MessageTilePO messageTilePO)
        {
            Tiles.Remove(messageTilePO);
        }

        public IMvxCommand<IDocumentPO> DocumentSelectedCommand { get; }

        public IBmmObservableCollection<ITilePO> Tiles { get; } =
            new BmmObservableCollection<ITilePO>();

        public async Task RefreshState()
        {
            foreach (var tile in Tiles.OfType<ITrackHolderPO>())
                await tile.RefreshState();
        }
    }
}