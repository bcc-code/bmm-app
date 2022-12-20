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
            var continueListeningTilePOs = tilesCollection
                .TileElements
                .Select(doc => tilePOFactory.Create(optionClickedCommand, doc));

            Tiles.AddRange(continueListeningTilePOs);
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