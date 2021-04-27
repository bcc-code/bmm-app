using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using Tavis.UriTemplates;

namespace BMM.Api.Implementation.Clients
{
    public class UsersClient : BaseClient, IUsersClient
    {
        public UsersClient(IRequestHandler handler, ApiBaseUri baseUri, ILogger logger)
            : base(handler, baseUri, logger)
        { }

        /// <summary>
        /// ToDo: This method is actually not maintained and never used. In case you decide to start using it you have to first understand how it works.
        /// </summary>
        public bool HasRole(User user, UserRole role)
        {
            return user.Roles.Contains(role.ToString()) ||

            // System internal dependencies
            role == UserRole.RoleTrackDelete &&
            HasRole(user, UserRole.RoleContributorDelete) ||
            role == UserRole.RoleTrackDelete &&
            HasRole(user, UserRole.RoleAlbumDelete) ||

            // Logical dependencies
            (role == UserRole.RoleAlbumRaw || role == UserRole.RoleAlbumSetCover) &&
            HasRole(user, UserRole.RoleAlbumSave) ||
            role == UserRole.RoleContributorSetCover &&
            HasRole(user, UserRole.RoleContributorSave) ||
            (role == UserRole.RoleTrackRaw || role == UserRole.RoleTrackSetCover) &&
            HasRole(user, UserRole.RoleTrackSave) ||
            (role == UserRole.RoleViewUploadedFiles || role == UserRole.RoleContentUnpublished) &&
            HasRole(user, UserRole.RoleTrackAddFile) ||

            // User defined dependencies
            (role == UserRole.RoleAlbumSave || role == UserRole.RoleAlbumDelete) &&
            HasRole(user, UserRole.RoleAlbumManager) ||
            (role == UserRole.RoleContributorSave || role == UserRole.RoleContributorDelete) &&
            HasRole(user, UserRole.RoleContributorManager) ||
            (role == UserRole.RoleTrackSave || role == UserRole.RoleTrackDelete || role == UserRole.RoleTrackAddFile || role == UserRole.RoleTrackMoveLanguage) &&
            HasRole(user, UserRole.RoleTrackManager) ||
            (role == UserRole.RoleContentUnpublished || role == UserRole.RoleAlbumManager || role == UserRole.RoleContributorManager ||
             role == UserRole.RoleTrackManager || role == UserRole.RoleUserAdministrator) &&
            HasRole(user, UserRole.RoleAdministrator);
        }

        public Task<User> GetCurrentLoggedInUser()
        {
            var uri = new UriTemplate(ApiUris.CurrentUser);
            var request = BuildRequest(uri, HttpMethod.Get);
            return RequestHandler.GetResolvedResponse<User>(request);
        }

        public Task CreateCurrentUser()
        {
            var uri = new UriTemplate(ApiUris.CurrentUser);
            var request = BuildRequest(uri, HttpMethod.Put);
            return RequestHandler.GetResponse(request);
        }

        public async Task<User> Login(string accessToken)
        {
            var uri = new UriTemplate(ApiUris.LoginJwt);

            return await RequestLogin(BuildRequest(uri, HttpMethod.Post, new {Token = accessToken}));
        }

        public async Task<User> Login(string username, string password)
        {
            var uri = new UriTemplate(ApiUris.Login);
            return await RequestLogin(BuildRequest(uri, HttpMethod.Post, new {Username = username, Password = password}));
        }

        private async Task<User> RequestLogin(IRequest buildRequestAction)
        {
            var request = buildRequestAction;

            try
            {
                return await RequestHandler.GetResolvedResponse<User>(request);
            }
            catch (UnauthorizedException ex)
            {
                return null;
            }
        }
    }
}