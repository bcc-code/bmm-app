using System;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Messages
{
    public class CloseAllSpecificViewModelsMessage : MvxMessage
    {
        public CloseAllSpecificViewModelsMessage(Type vmType, object sender) : base(sender)
        {
            VmType = vmType;
        }

        public Type VmType { get; }
    }
}