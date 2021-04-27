using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface IApiInfoClient
    {
        /// <summary>Gets information about the API.</summary>
        /// <returns>The API information.</returns>
        Task<ApiInfo> GetInfo();
    }
}