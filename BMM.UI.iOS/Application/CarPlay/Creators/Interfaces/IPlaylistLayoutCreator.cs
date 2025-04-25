using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IPlaylistLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, int playlistId, string name);
}