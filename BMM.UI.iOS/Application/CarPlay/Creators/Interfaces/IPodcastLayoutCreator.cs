using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IPodcastLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController, int podcastId, string name);
}