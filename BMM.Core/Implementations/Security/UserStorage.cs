using System.Threading.Tasks;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;

namespace BMM.Core.Implementations.Security
{
    /// <summary>
    /// Basic implementation of IUserStorage
    /// </summary>
    public class UserStorage : IUserStorage
    {
        private readonly ISecureStorageProxy _secureStorageProxy;
        private User _currentUser;
        private readonly object _getCurrentUserLocker;

        public UserStorage(ISecureStorageProxy secureStorageProxy)
        {
            _secureStorageProxy = secureStorageProxy;
            _getCurrentUserLocker = new object();
        }
        
        private User CurrentUser
        {
            get
            {
                lock (_getCurrentUserLocker)
                {
                    if (_currentUser == null)
                        _currentUser = _secureStorageProxy.GetAsync<User>(StorageKeys.CurrentUser).GetAwaiter().GetResult();

                    return _currentUser;
                }
            }
        }

        public async Task StoreUser(User user)
        {
            await _secureStorageProxy.SetAsync(StorageKeys.CurrentUser, user);
            _currentUser = user;
        }

        public bool HasUser() => GetUser() != null;
        public User GetUser() => CurrentUser;
        
        public void RemoveUser()
        {
            _secureStorageProxy.Remove(StorageKeys.CurrentUser);
            _currentUser = null;
        }
    }
}