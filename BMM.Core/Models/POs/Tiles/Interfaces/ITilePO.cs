using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.Core.Models.POs.Tiles.Interfaces
{
    public interface ITilePO : IDocumentPO
    {
    }
    
    public interface ITilePO<TTile> : ITilePO where TTile : Document
    {
        TTile Tile { get; }
    }
}