using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Badge;

public interface IBadgeService
{
    Task<bool> ShouldShowBadgeFor(Track track, int? podcastId);
}