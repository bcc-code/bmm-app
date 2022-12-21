using System;
using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Storage;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using BMM.Core.Models.POs.Tiles.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.Tiles
{
    public class MessageTilePO : DocumentPO, ITilePO<MessageTile>
    {
        public MessageTilePO(
            MessageTile messageTile,
            IDeepLinkHandler deepLinkHandler,
            IUriOpener uriOpener) : base(messageTile)
        {
            Tile = messageTile;
            BottomButtonClickedCommand = new ExceptionHandlingCommand(() =>
            {
                if (string.IsNullOrEmpty(messageTile.ButtonUrl))
                {
                    var dismissedMessageTilesIds = AppSettings.DismissedMessageTilesIds;
                    dismissedMessageTilesIds.Add(Tile.Id);
                    AppSettings.DismissedMessageTilesIds = dismissedMessageTilesIds;
                    RemoveItemFromObservableCollection?.Invoke(this);
                }
                else
                {
                    var uri = new Uri(messageTile.ButtonUrl);

                    if (deepLinkHandler.IsBmmUrl(uri))
                        deepLinkHandler.OpenFromInsideOfApp(uri, PlaybackOrigins.Tile);
                    else
                        uriOpener.OpenUri(uri);
                }
                
                return Task.CompletedTask;
            });
        }

        public IMvxCommand BottomButtonClickedCommand { get; }
        public MessageTile Tile { get; }
        public Action<MessageTilePO> RemoveItemFromObservableCollection { get; set; }
    }
}