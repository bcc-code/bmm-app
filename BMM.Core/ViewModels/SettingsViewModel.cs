using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
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
using BMM.Core.Messages;
using BMM.Core.Models;
using BMM.Core.ViewModels.Base;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
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
        private SelectableListItem _externalStorage;

        private List<IListItem> _listItems = new List<IListItem>();
        private string _profilePictureUrl;

        public List<IListItem> ListItems { get => _listItems; set => SetProperty(ref _listItems, value); }

        public IMvxCommand ItemSelectedCommand =>
            new MvxCommand<IListItem>(item => (item as IListContentItem)?.OnSelected?.Execute());

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
            IMvxLanguageBinder textSource = null)
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

            if (textSource != null)
            {
                TextSource = textSource;
            }

            _storageManager.Storages.CollectionChanged += Storages_CollectionChanged;

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "TextSource")
                {
                    BuildSections();
                }
            };

            _messenger.Subscribe<SelectedStorageChangedMessage>(message => { ChangeStorageText(message.FileStorage); }, MvxReference.Strong);
        }

        private void Storages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BuildSections();
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
            await BuildSections();
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
                    Text = TextSource.GetText("Logout"),
                    LogoutCommand = new ExceptionHandlingCommand(async () => await Logout()),
                    Title = TextSource.GetText("LoggedInAs"),
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
                new SectionHeader {ShowDivider = false, Title = TextSource.GetText("HeadlineSettings")},
                new CheckboxListItem
                {
                    Title = TextSource.GetText("OptionAutoplayHeader"),
                    Text = TextSource.GetText("OptionAutoplayText"),
                    IsChecked = await _settingsStorage.GetAutoplayEnabled(),
                    OnChanged = isChecked => _settingsStorage.SetAutoplayEnabled(isChecked)
                },
                new CheckboxListItem
                {
                    Title = TextSource.GetText("OptionStreakHiddenHeader"),
                    Text = TextSource.GetText("OptionStreakHiddenText"),
                    IsChecked = await _settingsStorage.GetStreakHidden(),
                    OnChanged = isChecked => _settingsStorage.SetStreakHidden(isChecked)
                },
                new CheckboxListItem
                {
                    Title = TextSource.GetText("OptionDownloadMobileNetworkHeader"),
                    Text = TextSource.GetText("OptionDownloadMobileNetworkText"),
                    IsChecked = await _networkSettings.GetMobileNetworkDownloadAllowed(),
                    OnChanged = mobileNetworkDownloadAllowed =>
                    {
                        _settingsStorage.SetMobileNetworkDownloadAllowed(mobileNetworkDownloadAllowed);
                        _mediaDownloader.SynchronizeOfflineTracks();
                        _messenger.Publish(new MobileNetworkDownloadAllowedChangeMessage(this, mobileNetworkDownloadAllowed));
                    }
                },
                new CheckboxListItem
                {
                    Title = TextSource.GetText("OptionPushNotifications"),
                    Text = TextSource.GetText("OptionPushNotificationsSubtitle"),
                    IsChecked = await _networkSettings.GetPushNotificationsAllowed(),
                    OnChanged = notificationsEnabled =>
                    {
                        _settingsStorage.SetPushNotificationsAllowed(notificationsEnabled);
                        _messenger.Publish(new PushNotificationsStatusChangedMessage(this, notificationsEnabled));
                    }
                }
            };

            if (_storageManager.HasMultipleStorageSupport)
            {
                _externalStorage = new SelectableListItem
                {
                    Title = TextSource.GetText("OptionExternalStorage"),
                    Text = CurrentStorageInfo(_storageManager.SelectedStorage),
                    OnSelected = _navigationService.NavigateCommand<StorageManagementViewModel>()
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

        private string CurrentStorageInfo(IFileStorage storage)
        {
            var gigabytesString = ByteToStringHelper.BytesToMegaBytes(storage.UsableSpace);

            return storage.StorageKind == StorageKind.Internal
                ? TextSource.GetText("OptionMBInternalText", gigabytesString)
                : TextSource.GetText("OptionMBExternalText", gigabytesString);
        }

        private async Task<List<IListItem>> BuildGeneralSection()
        {
            return new List<IListItem>
            {
                new SectionHeader {Title = TextSource.GetText("HeadlineGeneral")},
                new SelectableListItem
                {
                    Title = TextSource.GetText("OptionLanguageAppHeader"),
                    Text = TextSource.GetText("OptionLanguageAppText"),
                    OnSelected = _navigationService.NavigateCommand<LanguageAppViewModel>()
                },
                new SelectableListItem
                {
                    Title = TextSource.GetText("OptionLanguageContentHeader"),
                    Text = TextSource.GetText("OptionLanguageContentText"),
                    OnSelected = _navigationService.NavigateCommand<LanguageContentViewModel>()
                }
            };
        }

        private async Task<List<IListItem>> BuildAboutSection()
        {
            var items = new List<IListItem>
            {
                new SectionHeader {Title = TextSource.GetText("HeadlineAbout")},
                new SelectableListItem
                {
                    Title = TextSource.GetText("OptionCopyrightHeader"),
                    Text = TextSource.GetText("OptionCopyrightText"),
                    OnSelected = _navigationService.NavigateCommand<CopyrightViewModel>()
                },
                new SelectableListItem
                {
                    Title = TextSource.GetText("OptionContactHeader"),
                    Text = TextSource.GetText("OptionContactText"),
                    OnSelected = new MvxAsyncCommand(_contacter.Contact)
                },
                new SelectableListItem
                {
                    Title = TextSource.GetText("OptionAppVersionHeader"),
                    Text = GlobalConstants.AppVersion,
                    OnSelected = new ExceptionHandlingCommand(async () => await ShowInfo())
                }
            };

            var analyticsIdentifier =  _userStorage.GetUser().AnalyticsIdentifier;
            items.Add(new SelectableListItem
            {
                Title = "Analytics Identifier",
                Text = analyticsIdentifier,
                OnSelected = new MvxCommand(() => _clipboard.CopyToClipboard(analyticsIdentifier))
            });

            if (_developerPermission.IsBmmDeveloper())
            {
                var firebaseToken = await _tokenProvider.GetToken();
                items.Add(new SelectableListItem
                {
                    Title = "Firebase Token",
                    Text = firebaseToken,
                    OnSelected = new MvxCommand(() => _clipboard.CopyToClipboard(firebaseToken))
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
            }

            return items;
        }

        private void CrashTheApp()
        {
            throw new Exception("Forcefully crash the app!");
        }

        private async Task Logout()
        {
            var result = await Mvx.IoCProvider.Resolve<IUserDialogs>().ConfirmAsync(TextSource.GetText("LogoutConfirm"));
            if (!result) return;

            await _logoutService.PerformLogout();
            await _appNavigator.NavigateToLogin(false);

            // Event is fired after UI updated to ensure depending UI changes
            // are not called too early
            _messenger.Publish(new LoggedOutMessage(this));
        }

        private async Task ShowInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(TextSource.GetText("AppInfoAppVersion") + GlobalConstants.AppVersion);
            sb.AppendLine(TextSource.GetText("AppInfoManufacturer") + _deviceInfo.Manufacturer);
            sb.AppendLine(TextSource.GetText("AppInfoDeviceModel") + _deviceInfo.Model);
            sb.AppendLine(TextSource.GetText("AppInfoDevicePlatform") + _deviceInfo.Platform);
            sb.AppendLine(TextSource.GetText("AppInfoDeviceVersion") + _deviceInfo.VersionString);

            await Mvx.IoCProvider.Resolve<IUserDialogs>()
                .AlertAsync(new AlertConfig
                {
                    Title = TextSource.GetText("AppInfoTitle"),
                    Message = sb.ToString()
                });
        }
    }
}