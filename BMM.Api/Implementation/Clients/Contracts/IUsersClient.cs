using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Api.Implementation.Clients.Contracts
{
    public enum UserRole
    {
        RoleContentUnpublished,

        RoleUser,

        RoleAlbumRaw,
        RoleAlbumSetCover,
        RoleAlbumSave,
        RoleAlbumDelete,

        RoleContributorSetCover,
        RoleContributorSave,
        RoleContributorDelete,

        RoleTrackRaw,
        RoleTrackSetCover,
        RoleTrackSave,
        RoleTrackAddFile,
        RoleTrackMoveLanguage,
        RoleTrackDelete,

        RoleViewUploadedFiles,

        RoleUserAdministrator,

        // User defined
        RoleAlbumManager,

        RoleContributorManager,
        RoleTrackManager,
        RoleAdministrator,

        RoleFeaturePreview
    }

    public interface IUsersClient
    {
        Task<User> Login(string accessToken);

        /// <summary>Logs in the specified user.</summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The logged in user if successfull, otherwise null.</returns>
        Task<User> Login(string username, string password);

        /// <summary>
        /// Fetches the current logged in user with the bearer token because we do not get the
        /// user with the Open Id Connect Login
        /// </summary>
        /// <returns>The current logged in user</returns>
        Task<User> GetCurrentLoggedInUser();

        /// <summary>
        /// Starts the process of creating a new user user. Should be called when <see cref="GetCurrentLoggedInUser"/>
        /// returns a 417 (ExpectationFailed) status code
        /// </summary>
        /// <returns></returns>
        Task CreateCurrentUser();

        bool HasRole(User user, UserRole role);
    }
}