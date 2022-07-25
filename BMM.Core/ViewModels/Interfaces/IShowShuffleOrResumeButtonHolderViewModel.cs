namespace BMM.Core.ViewModels.Interfaces
{
    public interface IShowShuffleOrResumeButtonHolderViewModel : IBaseViewModel
    {
        bool ShowShuffleOrResumeButton { get; }
        string ShuffleOrResumeText { get; }
    }
}