using BMM.Core.ViewModels.Base;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class CellWrapperViewModel<T>: MvxViewModel
    {
        public T Item { get; set; }

        public BaseViewModel ViewModel { get; set; }

        public CellWrapperViewModel(T item, BaseViewModel viewModel)
        {
            Item = item;
            ViewModel = viewModel;
        }
    }
}