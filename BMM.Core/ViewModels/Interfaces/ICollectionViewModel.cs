using BMM.Core.Helpers.Interfaces;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface ICollectionViewModel<TItem>
    {
        IBmmObservableCollection<TItem> CollectionItems { get; }
        TItem SelectedCollectionItem { get; set; }
    }
}