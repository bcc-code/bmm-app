using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BMM.Api.Implementation.Models;
using IdentityModel;

namespace BMM.Core.Implementations.Security
{
    public class ClaimUserInformationExtractor : IClaimUserInformationExtractor
    {
        private const string PersonIdClaimName = "https://login.bcc.no/claims/personId";

        public User ExtractUser(ClaimsPrincipal claims)
        {
            return ExtractUser(claims.Claims);
        }

        public User ExtractUser(IEnumerable<Claim> claims)
        {
            var claimsList = claims.ToArray();
            var personIdString = GetClaimValueByClaimName(claimsList, PersonIdClaimName);
            var firstName = GetClaimValueByClaimName(claimsList, JwtClaimTypes.GivenName);
            var lastName = GetClaimValueByClaimName(claimsList, JwtClaimTypes.FamilyName);
            var picture = GetClaimValueByClaimName(claimsList, JwtClaimTypes.Picture);
            var subject = GetClaimValueByClaimName(claimsList, JwtClaimTypes.Subject);

            var personId = int.Parse(personIdString);
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                PersonId = personId,
                ProfileImage = picture,
                AnalyticsIdentifier = CreateAnalyticsIdentifier(personIdString, subject)
            };

            return user;
        }

        private string GetClaimValueByClaimName(IEnumerable<Claim> claims, string claimName)
        {
            var claimValue = claims.FirstOrDefault(c => c.Type == claimName)?.Value;
            if (claimValue == null)
                throw new Exception($"Could not parse claim with name: {claimName} from id_token claims");

            return claimValue;
        }

        private string CreateAnalyticsIdentifier(string personId, string subject)
        {
            var stringToHash = personId + subject;
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
            var builder = new StringBuilder();
            foreach (var b in hash)
            {
                builder.Append(b.ToString("X2"));
            }

            return builder.ToString();
        }
    }
}