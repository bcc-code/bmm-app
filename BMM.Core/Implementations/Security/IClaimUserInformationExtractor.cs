using System.Collections.Generic;
using System.Security.Claims;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Security
{
    public interface IClaimUserInformationExtractor
    {
        User ExtractUser(IEnumerable<Claim> claims);
    }
}