using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IFollowedPodcastsContentLayoutCreator
{
    Task<CPListTemplate> Create();
}