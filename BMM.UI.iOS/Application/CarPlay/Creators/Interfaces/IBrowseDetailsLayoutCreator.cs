using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IBrowseDetailsLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, string browsePath, string title);
}