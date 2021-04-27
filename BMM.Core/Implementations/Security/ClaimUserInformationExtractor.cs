using System;
using System.Linq;
using System.Security.Claims;
using BMM.Api.Implementation.Models;
using IdentityModel;

namespace BMM.Core.Implementations.Security
{
    public class ClaimUserInformationExtractor : IClaimUserInformationExtractor
    {
        private const string PersonIdClaimName = "https://login.bcc.no/claims/personId";

        public User ExtractUser(ClaimsPrincipal claims)
        {
            var personIdString = GetClaimValueByClaimName(claims, PersonIdClaimName);
            var firstName = GetClaimValueByClaimName(claims, JwtClaimTypes.GivenName);
            var lastName = GetClaimValueByClaimName(claims, JwtClaimTypes.FamilyName);
            var picture = GetClaimValueByClaimName(claims, JwtClaimTypes.Picture);

            var personId = int.Parse(personIdString);
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                PersonId = personId,
                ProfileImage = picture
            };

            return user;
        }

        private string GetClaimValueByClaimName(ClaimsPrincipal claims, string claimName)
        {
            var claimValue = claims.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
            if (claimValue == null)
                throw new Exception($"Could not parse claim with name: {claimName} from id_token claims");

            return claimValue;
        }
    }
}