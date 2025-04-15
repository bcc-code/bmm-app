using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IPlaylistsLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController);
}