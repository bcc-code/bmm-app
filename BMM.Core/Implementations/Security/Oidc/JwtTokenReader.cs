using System;
using System.IdentityModel.Tokens.Jwt;
using BMM.Core.Implementations.Security.Oidc.Interfaces;

namespace BMM.Core.Implementations.Security.Oidc
{
    public class JwtTokenReader : IJwtTokenReader
    {
        public DateTime GetExpirationTime(string accessToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenInfo = jwtSecurityTokenHandler.ReadToken(accessToken);
            return tokenInfo.ValidTo;
        }
    }
}