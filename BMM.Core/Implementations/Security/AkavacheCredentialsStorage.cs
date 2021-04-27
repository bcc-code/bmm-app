using Akavache;
using BMM.Api.Framework;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.Security
{
    /// <summary>
    /// Basic implementation of ICredentialsStorage. If no credential store provided you can use this one.
    /// </summary>
    public class AkavacheCredentialsStorage : ICredentialsStorage
    {
        public async Task StoreToken(IToken token)
        {
            await BlobCache.Secure.SaveLogin(token.Username, token.AuthenticationToken);
        }

        public async Task<bool> HasToken()
        {
            try
            {
                await BlobCache.Secure.GetLoginAsync();
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public async Task<IToken> GetToken()
        {
            LoginInfo data = await BlobCache.Secure.GetLoginAsync();

            return new Token(data.UserName, data.Password);
        }

        public async Task RemoveToken()
        {
            await BlobCache.Secure.EraseLogin();
        }
    }
}