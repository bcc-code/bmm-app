using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IHomeLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController);
}