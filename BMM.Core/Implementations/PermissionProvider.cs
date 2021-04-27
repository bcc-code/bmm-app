using System.Linq;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Security;

namespace BMM.Core.Implementations
{
    public class PermissionProvider : IFeaturePreviewPermission, IDeveloperPermission
    {
        private readonly IUserStorage _userStorage;

        private User User => _userStorage.GetUser();

        public PermissionProvider(IUserStorage userStorage)
        {
            _userStorage = userStorage;
        }

        public bool IsFeaturePreviewEnabled()
        {
            return User != null && (User.Roles.Contains("ROLE_FEATURE_PREVIEW") || User.Roles.Contains("ROLE_ADMINISTRATOR"));
        }

        public bool IsBmmDeveloper()
        {
            #if DEBUG
                return true;
            #endif

            return User != null && User.Roles.Contains("ROLE_ADMINISTRATOR");
        }
    }
}
