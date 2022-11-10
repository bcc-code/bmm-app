using System.Threading.Tasks;

namespace BMM.Core.Models.POs.Base
{
    public interface ITrackHolderPO
    {
        Task RefreshState();
    }
}