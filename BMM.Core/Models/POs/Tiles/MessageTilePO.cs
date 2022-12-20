using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tiles.Interfaces;

namespace BMM.Core.Models.POs.Tiles
{
    public class MessageTilePO : DocumentPO, ITilePO<MessageTile>
    {
        public MessageTilePO(MessageTile messageTile) : base(messageTile)
        {
            Tile = messageTile;
        }

        public MessageTile Tile { get; }
    }
}