using System.Collections.Generic;
using System.Threading.Tasks;
using BMM.Core.Models.POs;

namespace BMM.Core.Implementations.Tracks.Interfaces
{
    public interface ITrackOptionsService
    {
        Task OpenOptions(IList<StandardIconOptionPO> optionsList);
    }
}