using System.Security.Claims;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Security
{
    public interface IClaimUserInformationExtractor
    {
        User ExtractUser(ClaimsPrincipal claims);
    }
}