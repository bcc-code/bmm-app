using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Badge;

public interface IBadgeService
{
    bool IsBadgeSet { get; }
    Task Set();
    Task Remove();
    Task VerifyBadge();
    event EventHandler BadgeChanged;
}