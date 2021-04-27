using BMM.Core.Models;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class AutomaticDownloadCellWrapperViewModel: SelectableCellWrapperViewModel<ValueHeaderItem<int>>
    {
        public AutomaticDownloadCellWrapperViewModel(ValueHeaderItem<int> item, BaseViewModel viewModel) : base(item, viewModel)
        {
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AutomaticDownloadViewModel.AutomaticallyDownloadedTracks))
                {
                    RaisePropertyChanged(() => IsSelected);
                }
            };
        }

        public override bool IsSelected => _viewModel.AutomaticallyDownloadedTracks == Item.Value;

        private AutomaticDownloadViewModel _viewModel => (AutomaticDownloadViewModel) ViewModel;
    }
}