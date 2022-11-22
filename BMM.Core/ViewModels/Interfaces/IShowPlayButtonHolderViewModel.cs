namespace BMM.Core.ViewModels.Interfaces
{
    public interface IShowPlayButtonHolderViewModel : IBaseViewModel
    {
        bool ShowPlayButton { get; }
        string PlayButtonText { get; }
    }
}