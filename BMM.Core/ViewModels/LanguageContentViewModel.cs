using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Languages;
using BMM.Core.Implementations.Region.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels.Base;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace BMM.Core.ViewModels
{
    public class LanguageContentViewModel : BaseViewModel
    {
        private readonly IContentLanguageManager _contentLanguageManager;
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IUserDialogs _userDialogs;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ICache _cache;
        private readonly IGlobalMediaDownloader _mediaDownloader;
        private readonly IFirebaseRemoteConfig _firebaseRemoteConfig;
        private readonly ICultureInfoRepository _cultureInfoRepository;

        public LanguageContentViewModel(
            IContentLanguageManager contentLanguageManager,
            IToastDisplayer toastDisplayer,
            IUserDialogs userDialogs,
            IExceptionHandler exceptionHandler,
            ICache cache,
            IGlobalMediaDownloader mediaDownloader,
            IFirebaseRemoteConfig firebaseRemoteConfig,
            ICultureInfoRepository cultureInfoRepository)
        {
            _contentLanguageManager = contentLanguageManager;
            _toastDisplayer = toastDisplayer;
            _userDialogs = userDialogs;
            _exceptionHandler = exceptionHandler;
            _cache = cache;
            _mediaDownloader = mediaDownloader;
            _firebaseRemoteConfig = firebaseRemoteConfig;
            _cultureInfoRepository = cultureInfoRepository;

            Languages = new MvxObservableCollection<CultureInfoLanguage>();
            _availableLanguages = new List<CultureInfoLanguage>();
        }

        private readonly List<CultureInfoLanguage> _availableLanguages;

        public MvxObservableCollection<CultureInfoLanguage> Languages { get; }

        public Action<int> LanguageAddedAdapterCallback;
        public Action<int> LanguageRemovedAdapterCallback;

        public override async Task Initialize()
        {
            await base.Initialize();
            await Load();
        }

        protected override void AttachEvents()
        {
            base.AttachEvents();
            Languages.CollectionChanged += LanguagesOnCollectionChanged;
        }

        protected override void DetachEvents()
        {
            base.DetachEvents();
            Languages.CollectionChanged -= LanguagesOnCollectionChanged;
        }

        private void LanguagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _exceptionHandler.HandleException(_cache.Clear());
            _exceptionHandler.HandleException(_contentLanguageManager.SetContentLanguages(Languages.Select(l => l.Name)));
            _exceptionHandler.HandleException(_mediaDownloader.InitializeCacheAndSynchronizeTracks());
        }

        private async Task Load()
        {
            try
            {
                // Init the languages, chosen by the user
                var contentLanguages = await _contentLanguageManager.GetContentLanguages();
                Languages.ReplaceWith(contentLanguages.Select(code => _cultureInfoRepository.GetCultureInfoLanguage(code)));

                RefreshAvailableContentLanguages();
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex);
            }
        }

        private void RefreshAvailableContentLanguages()
        {
            var languages = _firebaseRemoteConfig.SupportedContentLanguages;

            foreach (var languageIso in languages)
            {
                // Skip language-independent content. Everybody wants to listen to instrumental songs, right? So we add it as fixed value.
                if (languageIso == ContentLanguageManager.LanguageIndependentContent)
                    continue;

                _availableLanguages.Add(_cultureInfoRepository.GetCultureInfoLanguage(languageIso));
            }

            _availableLanguages.Sort((x, y) => string.CompareOrdinal(x.NativeName, y.NativeName));
        }

        private IMvxCommand _openSelectLanguageDialogCommand;

        public IMvxCommand OpenSelectLanguageDialogCommand => _openSelectLanguageDialogCommand ?? (_openSelectLanguageDialogCommand = new ExceptionHandlingCommand(OpenSelectLanguageDialog));

        private async Task OpenSelectLanguageDialog()
        {
            if (_availableLanguages.Count == 0)
            {
                await _toastDisplayer.WarnAsync(TextSource[Translations.LanguageContentViewModel_Toast_NoLanguagesAvailable]);
                return;
            }

            var actionSheet = new ActionSheetConfig()
                .SetTitle(TextSource[Translations.LanguageContentViewModel_Dialog_Title])
                .SetCancel(TextSource[Translations.LanguageContentViewModel_Dialog_Cancel]);

            var valueConverter = new LanguageNameValueConverter();
            foreach (var language in _availableLanguages)
            {
                if (!Languages.Contains(language))
                {
                    var lang = language;
                    actionSheet.AddHandled(
                        (string)valueConverter.Convert(language, null, null, null),
                        async () =>
                        {
                            Languages.Add(lang);
                            LanguageAddedAdapterCallback?.Invoke(Languages.Count - 1);
                        }
                    );
                }
            }

            _userDialogs.ActionSheet(actionSheet);
        }

        private MvxCommand<CultureInfoLanguage> _deleteCommand;

        public IMvxCommand DeleteCommand
        {
            get
            {
                _deleteCommand = _deleteCommand ?? new MvxCommand<CultureInfoLanguage>(DeleteAction);
                return _deleteCommand;
            }
        }

        protected void DeleteAction(CultureInfoLanguage item)
        {
            if (Languages.Count > 1)
                LanguageRemovedAdapterCallback?.Invoke(Languages.IndexOf(item));
            else
            {
                var valueConverter = new LanguageNameValueConverter();
                _userDialogs.Alert(TextSource[Translations.LanguageContentViewModel_DeleteLanguageMessage], TextSource.GetText(Translations.LanguageContentViewModel_DeleteLanguageTitle, (string)valueConverter.Convert(item, null, null, null)));
            }
        }
    }
}