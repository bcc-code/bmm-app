using BMM.Core.Helpers;
using BMM.Core.ViewModels.Base;
using MvvmCross.ViewModels;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Implementations.Languages;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class LanguageAppViewModel : BaseViewModel
    {
        private readonly IAppLanguageProvider _appLanguageProvider;
        private IMvxCommand _languageSelectedCommand;

        public LanguageAppViewModel(IAppLanguageProvider appLanguageProvider)
        {
            _appLanguageProvider = appLanguageProvider;
            Languages = new MvxObservableCollection<CultureInfo>();
        }

        public MvxObservableCollection<CultureInfo> Languages { get; }

        public IMvxCommand LanguageSelectedCommand
        {
            get
            {
                _languageSelectedCommand = _languageSelectedCommand ?? new MvxCommand<CultureInfo>(LanguageChanged);
                return _languageSelectedCommand;
            }
        }

        private string _currentLanguage;

        public string CurrentLanguage => _currentLanguage ?? (_currentLanguage = _appLanguageProvider.GetAppLanguage());

        public override async Task Initialize()
        {
            await base.Initialize();

            var languages = GlobalConstants.AvailableAppLanguages.Select(languageIso => new CultureInfo(languageIso)).ToList();
            languages.Sort((x, y) => string.CompareOrdinal(x.NativeName, y.NativeName));
            Languages.ReplaceWith(languages);
        }

        private void LanguageChanged(CultureInfo cultureInfo)
        {
            _appLanguageProvider.ChangeAppLanguage(cultureInfo);
            _currentLanguage = cultureInfo.TwoLetterISOLanguageName;
            RaisePropertyChanged(() => CurrentLanguage);
        }
    }
}