using System.ComponentModel;
using BMM.Core.Models;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class AutomaticDownloadCellWrapperViewModel : SelectableCellWrapperViewModel<ValueHeaderItem<int>>
    {
        public AutomaticDownloadCellWrapperViewModel(ValueHeaderItem<int> item, BaseViewModel viewModel) : base(item, viewModel)
        {
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModels.AutomaticDownloadViewModel.AutomaticallyDownloadedTracks))
                RaisePropertyChanged(() => IsSelected);
        }

        public override bool IsSelected => AutomaticDownloadViewModel.AutomaticallyDownloadedTracks == Item.Value;

        private AutomaticDownloadViewModel AutomaticDownloadViewModel => (AutomaticDownloadViewModel) ViewModel;

        public void Subscribe()
        {
            AutomaticDownloadViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        public void Unsubscribe()
        {
            AutomaticDownloadViewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        }
    }
}