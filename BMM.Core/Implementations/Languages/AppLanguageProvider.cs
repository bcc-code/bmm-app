using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Akavache;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Region.Interfaces;
using MvvmCross;
using MvvmCross.Plugin.JsonLocalization;

namespace BMM.Core.Implementations.Languages
{
    public class AppLanguageProvider : IAppLanguageProvider
    {
        private readonly IBlobCache _storage;
        private readonly ICultureInfoRepository _cultureInfoRepository;
        private readonly EnvironmentLanguageReader _environmentLanguageReader;

        public AppLanguageProvider(
            IBlobCache storage,
            ICultureInfoRepository cultureInfoRepository,
            EnvironmentLanguageReader environmentLanguageReader)
        {
            _storage = storage;
            _cultureInfoRepository = cultureInfoRepository;
            _environmentLanguageReader = environmentLanguageReader;
        }

        public void InitializeAtStartup(string language)
        {
            var culture = _cultureInfoRepository.Get(language);
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public void ChangeAppLanguage(CultureInfo culture)
        {
            var builder = (TextProviderBuilder)Mvx.IoCProvider.GetSingleton<IMvxTextProviderBuilder>();
            builder.LoadResources(culture.TwoLetterISOLanguageName);

            BlobCache.UserAccount.InsertObject(StorageKeys.LanguageApp, culture.TwoLetterISOLanguageName);

            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentUICulture = culture;
            Mvx.IoCProvider.Resolve<INotificationCenter>().RaiseAppLanguageChanged();
        }

        public string GetAppLanguage()
        {
            var language = _storage
                .GetOrCreateObject(StorageKeys.LanguageApp, GetEnvironmentLanguage)
                .Wait();

            if (!GlobalConstants.AvailableAppLanguages.Contains(language))
                language = GlobalConstants.AvailableAppLanguages.FirstOrDefault();

            return language;
        }

        private string GetEnvironmentLanguage()
        {
            return _environmentLanguageReader.ReadLanguageFromEnvironment(GlobalConstants.AvailableAppLanguages);
        }
    }
}