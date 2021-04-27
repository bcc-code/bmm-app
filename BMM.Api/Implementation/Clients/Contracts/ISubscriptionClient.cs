using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public interface ISubscriptionClient
    {
        Task<bool> Delete(Subscription subscription);

        Task<bool> Save(Subscription subscription);
    }
}