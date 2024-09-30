using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Helpers;
using BMM.Core.Implementations.UI;
using BMM.Core.Models.POs.Base;
using Microsoft.IdentityModel.Tokens;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs.InfoMessages
{
    public class InfoMessagePO : DocumentPO
    {
        public InfoMessagePO(
            InfoMessage infoMessage,
            IUriOpener uriOpener) : base(infoMessage)
        {
            InfoMessage = infoMessage;
            TapCommand = new ExceptionHandlingCommand(() =>
            {
                if (infoMessage.Link.IsNullOrEmpty())
                    return Task.CompletedTask;
                
                uriOpener.OpenUri(infoMessage.Link.ToUri());
                
                return Task.CompletedTask;
            });
        }
        
        public InfoMessage InfoMessage { get; }
        public IMvxAsyncCommand TapCommand { get; }
    }
}