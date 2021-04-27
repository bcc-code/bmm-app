using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.PostLoginActions
{
    public interface ICurrentUserLoader
    {
        /// <summary>
        /// Fetches the current user from the API who is used to enrich
        /// the data of the current saved user
        /// </summary>
        /// <param name="user">The user to be updated with the API user</param>
        Task TryToLoadUserFromApi(User user);

        /// <summary>
        /// Tries to create a user at the API and then begins to fetch it with a defined interval
        /// </summary>
        /// <param name="user">The user to be updated with the API user</param>
        Task CreateUserInApiAndTryToLoadIt(User user);
    }
}