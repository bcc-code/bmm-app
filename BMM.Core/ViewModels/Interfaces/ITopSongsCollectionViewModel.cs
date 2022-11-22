using MvvmCross.Commands;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface ITopSongsCollectionViewModel : IBaseViewModel
    {
        public string Name { get; set; }
        public string PageTitle { get; set; }
        IMvxAsyncCommand AddToFavouritesCommand { get; }
    }
}