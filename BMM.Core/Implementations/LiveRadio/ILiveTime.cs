using System;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.LiveRadio
{
    public interface ILiveTime
    {
        DateTime TimeOnServer { get; }

        void SetLiveInfo(LiveInfo liveInfo);
    }
}
