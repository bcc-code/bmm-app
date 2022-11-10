using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Api.Implementation.Models.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.Interfaces;
using BMM.Core.Implementations.Factories.ContinueListening;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Base.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.ContinueListening
{
    public class ContinueListeningCollectionPO : DocumentPO, ITrackHolderPO
    {
        public ContinueListeningCollectionPO(
            IContinueListeningTilePOFactory continueListeningTilePOFactory,
            IMvxCommand<IDocumentPO> documentSelectedCommand,
            IMvxAsyncCommand<Document> optionClickedCommand,
            ContinueListeningCollection continueListeningCollection) : base(continueListeningCollection)
        {
            DocumentSelectedCommand = documentSelectedCommand;
            var continueListeningTilePOs = continueListeningCollection
                .ContinueListeningElements
                .Select(cle => continueListeningTilePOFactory.Create(optionClickedCommand, (ContinueListeningTile)cle));

            ContinueListeningTiles.AddRange(continueListeningTilePOs);
        }

        public IMvxCommand<IDocumentPO> DocumentSelectedCommand { get; }

        public IBmmObservableCollection<ContinueListeningTilePO> ContinueListeningTiles { get; } =
            new BmmObservableCollection<ContinueListeningTilePO>();

        public async Task RefreshState()
        {
            foreach (var continueListeningTile in ContinueListeningTiles)
                await continueListeningTile.RefreshState();
        }
    }
}