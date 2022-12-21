using System;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tiles.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tiles
{
    public class VideoTilePO : DocumentPO, ITilePO<VideoTile>
    {
        public VideoTilePO(
            VideoTile videoTile,
            IDeepLinkHandler deepLinkHandler,
            IUriOpener uriOpener) : base(videoTile)
        {
            Tile = videoTile;
            BottomButtonClickedCommand = new ExceptionHandlingCommand(() =>
            {
                var uri = new Uri(videoTile.ButtonUrl);

                if (deepLinkHandler.IsBmmUrl(uri))
                    deepLinkHandler.OpenFromInsideOfApp(uri, PlaybackOrigins.Tile);
                else
                    uriOpener.OpenUri(uri);
                
                return Task.CompletedTask;
            });
        }

        public IMvxCommand BottomButtonClickedCommand { get; }
        public VideoTile Tile { get; }
    }
}