using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;

namespace BMM.Core.Implementations.Security
{
    /// <summary>
    /// Basic implementation of IUserStorage
    /// </summary>
    public class AkavacheUserStorage : IUserStorage
    {
        private User _currentUser;
        private readonly object _getCurrentUserLocker;

        private User CurrentUser
        {
            get
            {
                lock (_getCurrentUserLocker)
                {
                    if (_currentUser == null)
                    {
                        _currentUser = BlobCache.Secure.GetOrCreateObject<User>(StorageKeys.CurrentUser, () => null).Wait();
                    }

                    return _currentUser;
                }
            }
        }

        public AkavacheUserStorage()
        {
            _getCurrentUserLocker = new object();
        }

        public async Task StoreUser(User user)
        {
            await BlobCache.Secure.InsertObject(StorageKeys.CurrentUser, user);
            _currentUser = user;
        }

        public async Task<bool> HasUser()
        {
            try
            {
                var data = await BlobCache.Secure.GetObject<User>(StorageKeys.CurrentUser);
                return data != null;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public User GetUser()
        {
            return CurrentUser;
        }

        public async Task RemoveUser()
        {
            await BlobCache.Secure.InvalidateObject<User>(StorageKeys.CurrentUser);
            await BlobCache.Secure.Vacuum();
            _currentUser = null;
        }
    }
}