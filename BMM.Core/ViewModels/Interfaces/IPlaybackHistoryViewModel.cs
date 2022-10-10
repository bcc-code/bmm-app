using BMM.Core.Implementations.TrackInformation.Strategies;

namespace BMM.Core.ViewModels.Interfaces
{
	public interface IPlaybackHistoryViewModel : IBaseViewModel
	{
		ITrackInfoProvider TrackInfoProvider { get; }
        bool HasAnyEntry { get; }
	}
}