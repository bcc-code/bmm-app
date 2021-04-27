using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface ILiveClient
    {
        Task<LiveInfo> GetInfo();
    }
}