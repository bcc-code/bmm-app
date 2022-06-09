using System;

namespace BMM.Core.Implementations.Security.Oidc.Interfaces
{
    public interface IJwtTokenReader
    {
        DateTime GetExpirationTme(string accessToken);
    }
}