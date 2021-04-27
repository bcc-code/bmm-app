using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public abstract class SelectableCellWrapperViewModel<T>: CellWrapperViewModel<T>
    {
        public abstract bool IsSelected { get; }

        public SelectableCellWrapperViewModel(T item, BaseViewModel viewModel) : base(item, viewModel)
        {
        }
    }
}