using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Tiles;
using BMM.Core.Models.POs.Tiles.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Implementations.Factories.ContinueListening
{
    public interface ITilePOFactory
    {
        ITilePO Create(IMvxAsyncCommand<Document> optionsClickedCommand, Document tile);
    }
}