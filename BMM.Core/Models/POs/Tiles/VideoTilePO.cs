using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tiles.Interfaces;

namespace BMM.Core.Models.POs.Tiles
{
    public class VideoTilePO : DocumentPO, ITilePO<VideoTile>
    {
        public VideoTilePO(VideoTile videoTile) : base(videoTile)
        {
            Tile = videoTile;
        }

        public VideoTile Tile { get; }
    }
}