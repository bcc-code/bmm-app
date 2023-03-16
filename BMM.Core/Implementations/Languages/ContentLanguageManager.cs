using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Storage;
using BMM.Core.Messages;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Languages
{
    public class ContentLanguageManager : IContentLanguageManager
    {
        /// <summary>
        /// zxx is used as identifier for content that does not have a language (e.g. instrumental).
        /// </summary>
        public const string LanguageIndependentContent = "zxx";
        public const string NorwegianIsoCode = "nb";
        public const string EnglishIsoCode = "en";

        private readonly IEnumerable<string> _hiddenLanguages = new List<string> {LanguageIndependentContent};
        private readonly IMvxMessenger _messenger;
        private readonly IFirebaseRemoteConfig _remoteConfig;
        private readonly EnvironmentLanguageReader _environmentLanguageReader;

        private readonly IBlobCache _storage;
        private IList<string> _contentLanguages = new List<string>();

        public ContentLanguageManager(IBlobCache storage, IMvxMessenger messenger, IFirebaseRemoteConfig remoteConfig, EnvironmentLanguageReader environmentLanguageReader)
        {
            _storage = storage;
            _messenger = messenger;
            _remoteConfig = remoteConfig;
            _environmentLanguageReader = environmentLanguageReader;
        }

        private string[] DefaultLanguages
        {
            get
            {
                var supportedLanguages = _remoteConfig.SupportedContentLanguages;
                var firstLanguage = _environmentLanguageReader.ReadLanguageFromEnvironment(supportedLanguages);

                if (firstLanguage == NorwegianIsoCode)
                    return new[] {firstLanguage};

                const string secondLanguage = NorwegianIsoCode;
                return new[] {firstLanguage, secondLanguage};
            }
        }

        public async Task<IEnumerable<string>> GetContentLanguages()
        {
            if (!_contentLanguages.Any())
                await GetAndSetLanguagesFromLocalStorage();

            return _contentLanguages.Except(_hiddenLanguages);
        }

        public async Task<IEnumerable<string>> GetContentLanguagesIncludingHidden()
        {
            if (!_contentLanguages.Any())
                await GetAndSetLanguagesFromLocalStorage();

            return _contentLanguages;
        }

        public async Task SetContentLanguages(IEnumerable<string> languages)
        {
            var langArr = languages.ToArray();
            SetContentLanguagesWithHiddenLanguages(langArr);
            AppSettings.LanguageContent = langArr;
            _messenger.Publish(new ContentLanguagesChangedMessage(this, langArr));
        }

        private async Task GetAndSetLanguagesFromLocalStorage()
        {
            string[] languages = AppSettings.LanguageContent ?? DefaultLanguages;
            SetContentLanguagesWithHiddenLanguages(languages);
        }

        private void SetContentLanguagesWithHiddenLanguages(IEnumerable<string> contentLanguages)
        {
            _contentLanguages = contentLanguages.Concat(_hiddenLanguages).Distinct().ToList();
        }
    }
}