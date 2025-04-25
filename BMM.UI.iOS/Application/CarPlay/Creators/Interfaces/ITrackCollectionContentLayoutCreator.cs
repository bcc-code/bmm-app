using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface ITrackCollectionContentLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, string title, int trackCollectionId);
}