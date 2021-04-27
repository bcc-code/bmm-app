using System.Globalization;
using BMM.Core.ViewModels.Base;

namespace BMM.Core.ViewModels
{
    public class LanguageCellWrapperViewModel: SelectableCellWrapperViewModel<CultureInfo>
    {
        private LanguageAppViewModel _viewModel => (LanguageAppViewModel) ViewModel;

        public LanguageCellWrapperViewModel(CultureInfo item, BaseViewModel viewModel) : base(item, viewModel)
        {
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(LanguageAppViewModel.CurrentLanguage))
                {
                    RaisePropertyChanged(() => IsSelected);
                }
            };
        }

        public override bool IsSelected => _viewModel.CurrentLanguage.Equals(Item.TwoLetterISOLanguageName);
    }
}