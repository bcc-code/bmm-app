namespace BMM.Core.ViewModels.Interfaces
{
	public interface IPlaybackHistoryViewModel : IBaseViewModel
	{
        bool HasAnyEntry { get; }
	}
}