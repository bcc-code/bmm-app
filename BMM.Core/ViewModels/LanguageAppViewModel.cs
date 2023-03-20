using BMM.Core.Helpers;
using BMM.Core.ViewModels.Base;
using MvvmCross.ViewModels;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BMM.Core.Constants;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Region.Interfaces;
using MvvmCross.Commands;

namespace BMM.Core.ViewModels
{
    public class LanguageAppViewModel : BaseViewModel
    {
        private readonly IAppLanguageProvider _appLanguageProvider;
        private readonly ICultureInfoRepository _cultureInfoRepository;
        private IMvxCommand _languageSelectedCommand;

        public LanguageAppViewModel(
            IAppLanguageProvider appLanguageProvider,
            ICultureInfoRepository cultureInfoRepository)
        {
            _appLanguageProvider = appLanguageProvider;
            _cultureInfoRepository = cultureInfoRepository;
            Languages = new MvxObservableCollection<CultureInfoLanguage>();
        }

        public MvxObservableCollection<CultureInfoLanguage> Languages { get; }

        public IMvxCommand LanguageSelectedCommand
        {
            get
            {
                _languageSelectedCommand = _languageSelectedCommand ?? new MvxCommand<CultureInfoLanguage>(LanguageChanged);
                return _languageSelectedCommand;
            }
        }

        private string _currentLanguage;

        public string CurrentLanguage => _currentLanguage ??= _appLanguageProvider.GetAppLanguage();

        public override async Task Initialize()
        {
            await base.Initialize();

            var languages = GlobalConstants.AvailableAppLanguages.Select(languageIso => _cultureInfoRepository.GetCultureInfoLanguage(languageIso)).ToList();
            languages.Sort((x, y) => string.CompareOrdinal(x.NativeName, y.NativeName));
            Languages.ReplaceWith(languages);
        }

        private void LanguageChanged(CultureInfoLanguage cultureInfo)
        {
            _appLanguageProvider.ChangeAppLanguage(cultureInfo);
            _currentLanguage = cultureInfo.Name;
            RaisePropertyChanged(() => CurrentLanguage);
        }
    }
}