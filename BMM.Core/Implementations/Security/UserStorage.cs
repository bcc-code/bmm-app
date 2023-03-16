using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Storage;

namespace BMM.Core.Implementations.Security
{
    /// <summary>
    /// Basic implementation of IUserStorage
    /// </summary>
    public class UserStorage : IUserStorage
    {
        private User _currentUser;

        private User CurrentUser => _currentUser ??= AppSettings.CurrentUser;

        public void StoreUser(User user)
        {
            AppSettings.CurrentUser = user;
            _currentUser = user;
        }

        public bool HasUser() => AppSettings.CurrentUser != null;
        public User GetUser() => CurrentUser;
        
        public void RemoveUser()
        {
            AppSettings.CurrentUser = null;
            _currentUser = null;
        }
    }
}