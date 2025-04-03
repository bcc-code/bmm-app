using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IFavouritesLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController);
}