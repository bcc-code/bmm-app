using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IBrowseLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController);
}