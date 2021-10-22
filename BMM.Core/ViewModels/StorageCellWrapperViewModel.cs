using BMM.Core.Implementations.FileStorage;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class StorageCellWrapperViewModel : SelectableCellWrapperViewModel<IFileStorage>
    {
        private StorageManagementViewModel StorageManagementViewModel => (StorageManagementViewModel)ViewModel;

        public StorageCellWrapperViewModel(IFileStorage item, BaseViewModel viewModel) : base(item, viewModel)
        {
            StorageManagementViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ViewModels.StorageManagementViewModel.SelectedStorage))
                {
                    RaisePropertyChanged(() => IsSelected);
                }
            };
        }

        public override bool IsSelected => StorageManagementViewModel.SelectedStorage == Item;
    }
}
