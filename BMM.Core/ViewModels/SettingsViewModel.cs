using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.DebugInfo.Interfaces;
using BMM.Core.GuardedActions.Settings.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Downloading;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Feedback;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FirebaseRemoteConfig;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.UI;
using BMM.Core.Implementations.UI.StyledText;
using BMM.Core.Implementations.UI.StyledText.Enums;
using BMM.Core.Messages;
using BMM.Core.Models;
using BMM.Core.Models.Parameters;
using BMM.Core.Models.POs.Other;
using BMM.Core.Translation;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private const string AnalyticsIdItemTitle = "Analytics ID";
        private const string FirebaseTokenItemTitle = "Firebase Token";
        private readonly IDeviceInfo _deviceInfo;
        private readonly INetworkSettings _networkSettings;
        private readonly ISettingsStorage _settingsStorage;
        private readonly INotificationSubscriptionTokenProvider _tokenProvider;
        private readonly IClipboardService _clipboard;
        private readonly IDeveloperPermission _developerPermission;
        private readonly IContacter _contacter;
        private readonly IAnalytics _analytics;
        private readonly IStorageManager _storageManager;
        private readonly ICache _cache;
        private readonly IGlobalMediaDownloader _mediaDownloader;
        private readonly IAppNavigator _appNavigator;
        private readonly ILogoutService _logoutService;
        private readonly IUriOpener _uriOpener;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IProfileLoader _profileLoader;
        private readonly IUserStorage _userStorage;
        private readonly IFirebaseRemoteConfig _remoteConfig;
        private readonly IFeatureSupportInfoService _featureSupportInfoService;
        private readonly INotificationPermissionService _notificationPermissionService;
        private readonly IChangeNotificationSettingStateAction _changeNotificationSettingStateAction;
        private SelectableListItem _externalStorage;

        private List<IListItem> _listItems = new List<IListItem>();
        private string _profilePictureUrl;
        private CheckboxListItemPO _pushNotificationCheckboxListItem;
        private StyledTextContainer _styledTextContainer;

        public List<IListItem> ListItems { get => _listItems; set => SetProperty(ref _listItems, value); }

        public IMvxCommand ItemSelectedCommand =>
            new MvxCommand<IListItem>(item => (item as IListContentItem)?.OnSelected?.Execute());

        public StyledTextContainer StyledTextContainer
        {
            get => _styledTextContainer;
            set => SetProperty(ref _styledTextContainer, value);
        }

        public SettingsViewModel(
            IDeviceInfo deviceInfo,
            INetworkSettings networkSettings,
            INotificationSubscriptionTokenProvider tokenProvider,
            IClipboardService clipboard,
            IDeveloperPermission developerPermission,
            IContacter contacter,
            IAnalytics analytics,
            ISettingsStorage settingsStorage,
            IStorageManager storageManager,
            ICache cache,
            IGlobalMediaDownloader mediaDownloader,
            IAppNavigator appNavigator,
            ILogoutService logoutService,
            IUriOpener uriOpener,
            IExceptionHandler exceptionHandler,
            IProfileLoader profileLoader,
            IUserStorage userStorage,
            IFirebaseRemoteConfig remoteConfig,
            IFeatureSupportInfoService featureSupportInfoService,
            INotificationPermissionService notificationPermissionService,
            IChangeNotificationSettingStateAction changeNotificationSettingStateAction)
        {
            _deviceInfo = deviceInfo;
            _networkSettings = networkSettings;
            _tokenProvider = tokenProvider;
            _clipboard = clipboard;
            _developerPermission = developerPermission;
            _contacter = contacter;
            _analytics = analytics;
            _settingsStorage = settingsStorage;
            _storageManager = storageManager;
            _cache = cache;
            _mediaDownloader = mediaDownloader;
            _appNavigator = appNavigator;
            _logoutService = logoutService;
            _uriOpener = uriOpener;
            _exceptionHandler = exceptionHandler;
            _profileLoader = profileLoader;
            _userStorage = userStorage;
            _remoteConfig = remoteConfig;
            _featureSupportInfoService = featureSupportInfoService;
            _notificationPermissionService = notificationPermissionService;
            _changeNotificationSettingStateAction = changeNotificationSettingStateAction;
            Messenger.Subscribe<SelectedStorageChangedMessage>(message => { ChangeStorageText(message.FileStorage); }, MvxReference.Strong);
        }

        public override void ViewCreated()
        {
            base.ViewCreated();
            PropertyChanged += OnPropertyChanged;
            ApplicationStateWatcher.ApplicationStateChanged += ApplicationStateChanged;
            _storageManager.Storages.CollectionChanged += OnStoragesCollectionChanged;
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            PropertyChanged -= OnPropertyChanged;
            ApplicationStateWatcher.ApplicationStateChanged -= ApplicationStateChanged;
            _storageManager.Storages.CollectionChanged -= OnStoragesCollectionChanged;
        }

        private async void ApplicationStateChanged(ApplicationState appState)
        {
            if (_pushNotificationCheckboxListItem == null)
                return;
            
            _pushNotificationCheckboxListItem.IsChecked = await GetIsPushNotificationAllowed();
        }
        
        private async void OnStoragesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await BuildSections();
        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TextSource))
                await BuildSections();
        }

        private void ChangeStorageText(IFileStorage fileStorage)
        {
            _externalStorage.Text = CurrentStorageInfo(fileStorage);
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            await BuildSections();
            _exceptionHandler.FireAndForget(LoadProfilePicture);
        }

        private async Task LoadProfilePicture()
        {
            var profile = await _profileLoader.LoadProfile();
            _profilePictureUrl = profile.Picture;
        }

        private async Task BuildSections()
        {
            var items = new List<List<IListItem>>
            {
                await BuildProfileSection(),
                await BuildSettingsSection(),
                await BuildGeneralSection(),
                await BuildAboutSection()
            };

            ListItems = items.SelectMany(x => x).ToList();
        }

        private async Task<List<IListItem>> BuildProfileSection()
        {
            return new List<IListItem>
            {
                new ProfileListItem
                {
                    Text = TextSource[Translations.SettingsViewModel_Logout],
                    LogoutCommand = new ExceptionHandlingCommand(async () => await Logout()),
                    Title = TextSource[Translations.SettingsViewModel_LoggedInAs],
                    UserProfileUrl = _profilePictureUrl,
                    Username = _userStorage.GetUser().FullName,
                    EditProfileCommand = new ExceptionHandlingCommand(async () => _uriOpener.OpenUri(new Uri(_remoteConfig.EditProfileUrl)))
                }
            };
        }

        private async Task<List<IListItem>> BuildSettingsSection()
        {
            var items = new List<IListItem>
            {
                new SectionHeader {ShowDivider = false, Title = TextSource[Translations.SettingsViewModel_HeadlineSettings]},
                new CheckboxListItemPO
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionAutoplayHeader],
                    Text = TextSource[Translations.SettingsViewModel_OptionAutoplayText],
                    IsChecked = await _settingsStorage.GetAutoplayEnabled(),
                    OnChanged = sender => _settingsStorage.SetAutoplayEnabled(sender.IsChecked)
                },
                new CheckboxListItemPO
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionStreakHeader],
                    Text = TextSource[Translations.SettingsViewModel_OptionStreakText],
                    IsChecked = !await _settingsStorage.GetStreakHidden(),
                    OnChanged = sender => _settingsStorage.SetStreakHidden(!sender.IsChecked)
                },
                new CheckboxListItemPO
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionDownloadMobileNetworkHeader],
                    Text = TextSource[Translations.SettingsViewModel_OptionDownloadMobileNetworkText],
                    IsChecked = await _networkSettings.GetMobileNetworkDownloadAllowed(),
                    OnChanged = sender =>
                    {
                        _settingsStorage.SetMobileNetworkDownloadAllowed(sender.IsChecked);
                        _mediaDownloader.SynchronizeOfflineTracks();
                        Messenger.Publish(new MobileNetworkDownloadAllowedChangeMessage(this, sender.IsChecked));
                    }
                }
            };

            _pushNotificationCheckboxListItem = new CheckboxListItemPO
            {
                Title = TextSource[Translations.SettingsViewModel_OptionPushNotifications],
                Text = TextSource[Translations.SettingsViewModel_OptionPushNotificationsSubtitle],
                IsChecked = await GetIsPushNotificationAllowed(),
                OnChanged = async sender =>
                {
                    await _changeNotificationSettingStateAction.ExecuteGuarded(sender.IsChecked);
                    sender.IsChecked = await GetIsPushNotificationAllowed();
                }
            };
            
            items.Add(_pushNotificationCheckboxListItem);
                
            if (_storageManager.HasMultipleStorageSupport)
            {
                _externalStorage = new SelectableListItem
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionExternalStorage],
                    Text = CurrentStorageInfo(_storageManager.SelectedStorage),
                    OnSelected = NavigationService.NavigateCommand<StorageManagementViewModel>()
                };
                items.Add(_externalStorage);
            }

            _analytics.LogEvent("Open Settings screen",
                new Dictionary<string, object>
                {
                    {"DownloadOverMobileNetworkAllowed", await _networkSettings.GetMobileNetworkDownloadAllowed()},
                    {"PushNotificationsAllowed", await _networkSettings.GetPushNotificationsAllowed()}
                });

            return items;
        }

        private async Task<bool> GetIsPushNotificationAllowed()
        {
            return await _networkSettings.GetPushNotificationsAllowed() && await _notificationPermissionService.CheckIsNotificationPermissionGranted();
        }

        private string CurrentStorageInfo(IFileStorage storage)
        {
            var gigabytesString = ByteToStringHelper.BytesToMegaBytes(storage.UsableSpace);

            return storage.StorageKind == StorageKind.Internal
                ? TextSource.GetText(Translations.SettingsViewModel_OptionMBInternalText, gigabytesString)
                : TextSource.GetText(Translations.SettingsViewModel_OptionMBExternalText, gigabytesString);
        }

        private async Task<List<IListItem>> BuildGeneralSection()
        {
            var generalSectionItems = new List<IListItem>
            {
                new SectionHeader {Title = TextSource[Translations.SettingsViewModel_HeadlineGeneral]},
                new SelectableListItem
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionLanguageAppHeader],
                    Text = TextSource[Translations.SettingsViewModel_OptionLanguageAppText],
                    OnSelected = NavigationService.NavigateCommand<LanguageAppViewModel>()
                },
                new SelectableListItem
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionLanguageContentHeader],
                    Text = TextSource[Translations.SettingsViewModel_OptionLanguageContentText],
                    OnSelected = NavigationService.NavigateCommand<LanguageContentViewModel>()
                }
            };
            
            generalSectionItems.AddIf(() => _featureSupportInfoService.SupportsDarkMode, new SelectableListItem
            {
                Title = TextSource[Translations.SettingsViewModel_ThemeHeader],
                Text = TextSource[Translations.SettingsViewModel_ThemeText],
                OnSelected = NavigationService.NavigateCommand<ThemeSettingsViewModel>()
            });
            
            generalSectionItems.AddIf(() => _featureSupportInfoService.SupportsSiri, new SelectableListItem
            {
                Title = TextSource[Translations.SettingsViewModel_SiriShortcutsHeader],
                Text = TextSource[Translations.SettingsViewModel_SiriShortcutsText],
                OnSelected = NavigationService.NavigateCommand<SiriShortcutsViewModel>()
            });

            return generalSectionItems;
        }

        private async Task<List<IListItem>> BuildAboutSection()
        {
            var items = new List<IListItem>
            {
                new SectionHeader {Title = TextSource[Translations.SettingsViewModel_HeadlineAbout]},
                new SelectableListItem
                {
                    Title = "TEST",
                    Text = "GOOGLE",
                    OnSelected = NavigationService.NavigateCommand<WebBrowserViewModel, IWebBrowserPrepareParams>(new WebBrowserPrepareParams()
                    {
                        Url = "https://app.bcc.media/embed/episode/1831/lesson/f634ddcd-d101-4a6b-9222-2f26e8c4e120?bg=true"
                    })
                },
                new SelectableListItem
                {
                    Title = TextSource[Translations.SettingsViewModel_UserVoiceHeader],
                    Text = TextSource[Translations.SettingsViewModel_UserVoiceText],
                    OnSelected = new MvxCommand(() => _uriOpener.OpenUri(new Uri(_remoteConfig.UserVoiceLink)))
                },
                new SelectableListItem
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionCopyrightHeader],
                    Text = TextSource[Translations.SettingsViewModel_OptionCopyrightText],
                    OnSelected = NavigationService.NavigateCommand<CopyrightViewModel>()
                },
                new SelectableListItem
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionContactHeader],
                    Text = TextSource[Translations.SettingsViewModel_OptionContactText],
                    OnSelected = new MvxAsyncCommand(_contacter.Contact)
                },
                new SelectableListItem
                {
                    Title = TextSource[Translations.SettingsViewModel_OptionAppVersionHeader],
                    Text = GlobalConstants.AppVersion,
                    OnSelected = new ExceptionHandlingCommand(async () => await ShowInfo())
                }
            };

            var analyticsId =  _userStorage.GetUser().AnalyticsId;
            items.Add(new SelectableListItem
            {
                Title = AnalyticsIdItemTitle,
                Text = analyticsId,
                OnSelected = new MvxCommand(() => _clipboard.CopyToClipboard(analyticsId, AnalyticsIdItemTitle))
            });

            if (_developerPermission.IsBmmDeveloper())
            {
                var firebaseToken = await _tokenProvider.GetToken();
                items.Add(new SelectableListItem
                {
                    Title = FirebaseTokenItemTitle,
                    Text = firebaseToken,
                    OnSelected = new MvxCommand(() => _clipboard.CopyToClipboard(firebaseToken, FirebaseTokenItemTitle))
                });
            }

            if (_developerPermission.IsBmmDeveloper())
            {
                items.Add(new SelectableListItem
                {
                    Title = "Clear cache",
                    Text = "Clear local cache and reload metadata from the server",
                    OnSelected = new ExceptionHandlingCommand(async () => await _cache.Clear())
                });

                items.Add(new SelectableListItem
                {
                    Title = "Crash the app",
                    Text = "Causes an exception that stops the app",
                    OnSelected = new MvxCommand(CrashTheApp)
                });

                if (_deviceInfo.IsIos)
                {
                    items.Add(new SelectableListItem
                    {
                        Title = "Show cached tracks",
                        Text = "Shows all cached Player Items and their size",
                        OnSelected = new MvxAsyncCommand(ShowCachedTracks)
                    });
                }
            }

            return items;
        }

        private async Task ShowCachedTracks()
        {
            await Mvx.IoCProvider.Resolve<IShowTracksCacheInfoAction>().ExecuteGuarded();
        }

        private void CrashTheApp()
        {
            throw new Exception("Forcefully crash the app!");
        }

        private async Task Logout()
        {
            var result = await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync(TextSource[Translations.SettingsViewModel_LogoutConfirm]);
            if (!result) return;

            await _logoutService.PerformLogout();
            await _appNavigator.NavigateToLogin(false);

            // Event is fired after UI updated to ensure depending UI changes
            // are not called too early
            Messenger.Publish(new LoggedOutMessage(this));
        }

        private async Task ShowInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(TextSource[Translations.SettingsViewModel_AppInfoAppVersion] + GlobalConstants.AppVersion);
            sb.AppendLine(TextSource[Translations.SettingsViewModel_AppInfoManufacturer] + _deviceInfo.Manufacturer);
            sb.AppendLine(TextSource[Translations.SettingsViewModel_AppInfoDeviceModel] + _deviceInfo.Model);
            sb.AppendLine(TextSource[Translations.SettingsViewModel_AppInfoDevicePlatform] + _deviceInfo.Platform);
            sb.AppendLine(TextSource[Translations.SettingsViewModel_AppInfoDeviceVersion] + _deviceInfo.VersionString);

            await Mvx.IoCProvider.Resolve<IUserDialogs>()
                .AlertAsync(new AlertConfig
                {
                    Title = TextSource[Translations.SettingsViewModel_AppInfoTitle],
                    Message = sb.ToString()
                });
        }
    }
}