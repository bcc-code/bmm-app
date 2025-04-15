using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IContributorLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, int contributorId, string name);
}