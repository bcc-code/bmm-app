using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BMM.Api.Abstraction;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.Player.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.Region.Interfaces;
using BMM.Core.Models.POs;
using BMM.Core.Models.POs.Base;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.Interfaces;

namespace BMM.Core.GuardedActions.Player
{
    public class PrepareAvailableTrackLanguageAction : GuardedAction, IPrepareAvailableTrackLanguageAction
    {
        private readonly IBMMLanguageBinder _bmmLanguageBinder;
        private readonly IContentLanguageManager _contentLanguageManager;
        private readonly ICultureInfoRepository _cultureInfoRepository;
        private LanguageNameValueConverter _languageNameValueConverter;
        
        private IChangeTrackLanguageViewModel ChangeTrackLanguageViewModel => this.GetDataContext();
        private LanguageNameValueConverter LanguageNameValueConverter => _languageNameValueConverter ??= new LanguageNameValueConverter();

        public PrepareAvailableTrackLanguageAction(
            IBMMLanguageBinder bmmLanguageBinder,
            IContentLanguageManager contentLanguageManager,
            ICultureInfoRepository cultureInfoRepository)
        {
            _bmmLanguageBinder = bmmLanguageBinder;
            _contentLanguageManager = contentLanguageManager;
            _cultureInfoRepository = cultureInfoRepository;
        }
        
        protected override async Task Execute()
        {
            var allAvailableLanguages = ChangeTrackLanguageViewModel
                .NavigationParameter
                .AvailableLanguages
                .ToList();

            string currentLanguage = ChangeTrackLanguageViewModel.NavigationParameter.Language;
            
            var selectablePOs = new List<BasePO>();
            selectablePOs.AddRange(await GetPreferredLanguageSection(allAvailableLanguages));

            var alreadyAddedLanguages = selectablePOs.OfType<StandardSelectablePO>().Select(s => s.Value);
            selectablePOs.AddRange(GetRestOfLanguages(allAvailableLanguages, alreadyAddedLanguages));

            selectablePOs
                .OfType<StandardSelectablePO>()
                .FirstOrDefault(l => l.Value == currentLanguage)
                .IfNotNull(l => l.IsSelected = true);
                
            ChangeTrackLanguageViewModel.AvailableLanguages.AddRange(selectablePOs);
        }

        private async Task<IEnumerable<BasePO>> GetPreferredLanguageSection(List<string> allAvailableLanguages)
        {
            var preferredLanguages = await _contentLanguageManager.GetContentLanguages();

            var preferredLanguagesToAdd = preferredLanguages
                .Where(allAvailableLanguages.Contains)
                .ToList();

            return GetDisplayableLanguages(Translations.ChangeTrackLanguageViewModel_PreferredLanguages, preferredLanguagesToAdd);
        }

        private IEnumerable<BasePO> GetRestOfLanguages(IEnumerable<string> allAvailableLanguages, IEnumerable<string> alreadyAddedLanguages)
        {
            var languagesToAdd = allAvailableLanguages
                .Where(x => !alreadyAddedLanguages.Contains(x))
                .ToList();

            return GetDisplayableLanguages(Translations.ChangeTrackLanguageViewModel_AllLanguages, languagesToAdd);
        }
        
        private IEnumerable<BasePO> GetDisplayableLanguages(string headerKey, List<string> languages)
        {
            if (!languages.Any())
                return Enumerable.Empty<BasePO>();
            
            var selectablePOs = new List<BasePO>
            {
                new HeaderPO(_bmmLanguageBinder[headerKey])
            };

            foreach (string lang in languages)
                selectablePOs.Add(new StandardSelectablePO(GetDisplayableLanguageName(lang), lang));

            return selectablePOs;
        }
        
        private string GetDisplayableLanguageName(string lang)
        {
            var cultureInfo = _cultureInfoRepository.GetCultureInfoLanguage(lang);

            return (string)LanguageNameValueConverter.Convert(cultureInfo,
                typeof(string),
                null,
                CultureInfo.InvariantCulture);
        }
    }
}