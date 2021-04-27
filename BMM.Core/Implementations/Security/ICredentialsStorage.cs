using BMM.Api.Framework;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    /// <summary>
    /// Stores tokens for use with the BMM API
    /// </summary>
    public interface ICredentialsStorage
    {
        /// <summary>
        /// Stores a token, overwriting the current token if it exists
        /// </summary>
        /// <param name="token">The token to be stored</param>
        /// <exception cref="ArgumentNullException">Null is not allowed. Use RemoveToken to unset the stored Token</exception>
        Task StoreToken(IToken token);

        /// <summary>
        /// Checks if a token is currently stored
        /// </summary>
        /// <returns>Whether or not the store contain a token right now</returns>
        Task<bool> HasToken();

        /// <summary>
        /// Returns the stored token
        /// </summary>
        /// <returns>The current token, or null if no token is stored</returns>
        Task<IToken> GetToken();

        /// <summary>
        /// Remove the current token from store
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if RemoveToken is called while no token is currently stored</exception>
        Task RemoveToken();
    }
}