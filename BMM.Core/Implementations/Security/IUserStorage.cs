using BMM.Api.Implementation.Models;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    /// <summary>
    /// Stores the user for using it's details with the BMM App
    /// </summary>
    public interface IUserStorage
    {
        /// <summary>
        /// Stores the user, overwriting the current user if it exists
        /// </summary>
        /// <param name="user">The user to be stored</param>
        /// <exception cref="ArgumentNullException">Null is not allowed. Use RemoveUser to unset the stored User</exception>
        void StoreUser(User user);

        /// <summary>
        /// Checks if a user is currently stored
        /// </summary>
        /// <returns>Whether or not the store contains a user right now</returns>
        bool HasUser();

        /// <summary>
        /// Returns the stored user
        /// </summary>
        /// <returns>The current user, or null if no user is stored</returns>
        User GetUser();

        /// <summary>
        /// Remove the current user
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if RemoveUser is called while no user is currently stored</exception>
        void RemoveUser();
    }
}