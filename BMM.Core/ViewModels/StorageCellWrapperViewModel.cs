using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class StorageCellWrapperViewModel : SelectableCellWrapperViewModel<IFileStorage>
    {
        private StorageManagementViewModel _viewModel => (StorageManagementViewModel)ViewModel;

        public StorageCellWrapperViewModel(IFileStorage item, BaseViewModel viewModel) : base(item, viewModel)
        {
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(StorageManagementViewModel.SelectedStorage))
                {
                    RaisePropertyChanged(() => IsSelected);
                }
            };
        }

        public override bool IsSelected => _viewModel.SelectedStorage == Item;
    }
}
