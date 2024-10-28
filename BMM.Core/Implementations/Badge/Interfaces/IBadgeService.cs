using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Badge;

public interface IBadgeService
{
    bool IsBadgeSet { get; }
    Task<bool> SetIfPossible();
    void Remove();
    Task VerifyBadge();
    event EventHandler BadgeChanged;
}