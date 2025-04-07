using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface ITrackCollectionContentLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, int trackCollectionId);
}