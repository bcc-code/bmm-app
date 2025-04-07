using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IAlbumLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, int albumId, string name);
}