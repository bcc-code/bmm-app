using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.InfoMessages
{
    public class InfoMessagePO : DocumentPO
    {
        public InfoMessagePO(InfoMessage infoMessage) : base(infoMessage)
        {
            InfoMessage = infoMessage;
        }
        
        public InfoMessage InfoMessage { get; }
    }
}