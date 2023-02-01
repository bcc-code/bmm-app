using BMM.Core.Helpers.Interfaces;
using BMM.Core.Interactions.Base;

namespace BMM.Core.ViewModels.Interfaces
{
    public interface ICollectionViewModel<TItem>
    {
        IBmmObservableCollection<TItem> CollectionItems { get; }
        TItem SelectedCollectionItem { get; set; }
        IBmmInteraction ResetInteraction { get; }
    }
}