using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.ContinueListening;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.ContinueListening
{
    public interface IContinueListeningTilePOFactory
    {
        ContinueListeningTilePO Create(IMvxAsyncCommand<Document> optionsClickedCommand, ContinueListeningTile continueListeningTile);
    }
}