using CarPlay;

namespace BMM.UI.iOS.CarPlay.Creators.Interfaces;

public interface IDownloadedContentLayoutCreator
{
    Task<CPListTemplate> Create(CPInterfaceController cpInterfaceController);
}