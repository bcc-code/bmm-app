using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.BibleStudy.Interfaces;
using BMM.Core.GuardedActions.Settings.Interfaces;
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
using BMM.Core.Models;
using BMM.Core.Models.POs.Other;
using BMM.Core.Test.Unit.ViewModels.Base;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Moq;
using MvvmCross.Localization;
using NSubstitute;
using NUnit.Framework;

namespace BMM.Core.Test.Unit.ViewModels
{
    [TestFixture]
    public class SettingsViewModelTests : BaseViewModelTests
    {
        private Mock<INetworkSettings> _networkSettings;
        private Mock<ISettingsStorage> _settingsStorage;
        private Mock<INotificationSubscriptionTokenProvider> _tokenProvider;
        private Mock<IClipboardService> _clipboard;
        private Mock<IDeveloperPermission> _developerPermission;
        private Mock<IContacter> _contacter;
        private Mock<IAnalytics> _analytics;
        private Mock<IStorageManager> _storageManager;
        private Mock<IMvxLanguageBinder> _textSource;
        private Mock<ICache> _cache;
        private Mock<IUserDialogs> _userDialogs;
        private Mock<IUserStorage> _userStorage;
        private Mock<IGlobalMediaDownloader> _mediaDownloader;
        private Mock<IAppNavigator> _appNavigator;
        private Mock<ILogoutService> _logoutService;
        private Mock<IUriOpener> _uriOpener;
        private Mock<IExceptionHandler> _exceptionHandler;
        private Mock<IProfileLoader> _profileLoader;
        private Mock<IFirebaseRemoteConfig> _remoteConfig;
        private Mock<IDeviceInfo> _deviceInfo;
        private Mock<IFeatureSupportInfoService> _featureSupportInfoService;
        private Mock<IFeaturePreviewPermission> _featurePreviewPermission;
        private Mock<INotificationPermissionService> _notificationPermissionService;
        private Mock<IChangeNotificationSettingStateAction> _changeNotificationSettingStateAction;
        private Mock<IResetAchievementAction> _resetAchievementAction;

        public override void SetUp()
        {
            base.SetUp();
            _deviceInfo = new Mock<IDeviceInfo>();

            _userDialogs = new Mock<IUserDialogs>();
            _userDialogs.Setup(x => x.ConfirmAsync(It.IsAny<string>(), null, null, null, null)).Returns(Task.FromResult(true));
            Ioc.RegisterSingleton(_userDialogs.Object);

            _userStorage = new Mock<IUserStorage>();
            _userStorage.Setup(x => x.GetUser())
                .Returns(new User
                {
                    FirstName = "Hans-Peter",
                    LastName = "Mueller",
                    ProfileImage = "some url"
                });
            Ioc.RegisterSingleton(_userStorage.Object);

            _cache = new Mock<ICache>();
            _cache.Setup(x => x.Clear()).Returns(Task.FromResult(default(object)));
            Ioc.RegisterSingleton(_cache.Object);

            _networkSettings = new Mock<INetworkSettings>();
            _networkSettings.Setup(x => x.GetMobileNetworkDownloadAllowed()).ReturnsAsync(true);
            _networkSettings.Setup(x => x.GetPushNotificationsAllowed()).ReturnsAsync(true);

            _settingsStorage = new Mock<ISettingsStorage>();
            _tokenProvider = new Mock<INotificationSubscriptionTokenProvider>();
            _clipboard = new Mock<IClipboardService>();
            _developerPermission = new Mock<IDeveloperPermission>();
            _contacter = new Mock<IContacter>();
            _analytics = new Mock<IAnalytics>();
            _storageManager = new Mock<IStorageManager>();
            _cache = new Mock<ICache>();
            _textSource = new Mock<IMvxLanguageBinder>();
            _mediaDownloader = new Mock<IGlobalMediaDownloader>();
            _appNavigator = new Mock<IAppNavigator>();

            _logoutService = new Mock<ILogoutService>();
            _logoutService.Setup(x => x.PerformLogout()).Returns(Task.CompletedTask);
            _uriOpener = new Mock<IUriOpener>();
            _exceptionHandler = new Mock<IExceptionHandler>();
            _profileLoader = new Mock<IProfileLoader>();
            _remoteConfig = new Mock<IFirebaseRemoteConfig>();
            _featureSupportInfoService = new Mock<IFeatureSupportInfoService>();
            _featurePreviewPermission = new Mock<IFeaturePreviewPermission>();
            _notificationPermissionService = new Mock<INotificationPermissionService>();
            _changeNotificationSettingStateAction= new Mock<IChangeNotificationSettingStateAction>();
            _resetAchievementAction = new Mock<IResetAchievementAction>();

            _textSource.Setup(x => x.GetText(It.IsAny<string>())).Returns(String.Empty);

            Mock<IFileStorage> defaultStorage = new Mock<IFileStorage>();

            _storageManager.Setup(x => x.Storages).Returns(new ObservableCollection<IFileStorage>() {defaultStorage.Object});
            _storageManager.Setup(x => x.SelectedStorage).Returns(defaultStorage.Object);
            _storageManager.Setup(x => x.HasMultipleStorageSupport).Returns(true);
        }

        public SettingsViewModel CreateSettingsViewModel()
        {
            var settingsViewModel = new SettingsViewModel(
                _deviceInfo.Object,
                _networkSettings.Object,
                _tokenProvider.Object,
                _clipboard.Object,
                _developerPermission.Object,
                _contacter.Object,
                _analytics.Object,
                _settingsStorage.Object,
                _storageManager.Object,
                _cache.Object,
                _mediaDownloader.Object,
                _appNavigator.Object,
                _logoutService.Object,
                _uriOpener.Object,
                _exceptionHandler.Object,
                _profileLoader.Object,
                _userStorage.Object,
                _remoteConfig.Object,
                _featureSupportInfoService.Object,
                _notificationPermissionService.Object,
                _changeNotificationSettingStateAction.Object,
                _resetAchievementAction.Object,
                _featurePreviewPermission.Object);

            settingsViewModel.TextSource = TextResource.Object;
            return settingsViewModel;
        }

        [Test]
        public void Ctor_ShouldNotFillSection()
        {
            // Arrange & Act
            var settingsViewModel = CreateSettingsViewModel();

            // Assert
            Assert.AreEqual(settingsViewModel.ListItems.Count, 0);
        }

        [Test]
        public async Task Init_ShouldFillSectionsCorrectly()
        {
            // Arrange
            var settingsViewModel = CreateSettingsViewModel();

            // Act
            await settingsViewModel.Initialize();

            // Assert
            Assert.GreaterOrEqual(settingsViewModel.ListItems.Count, 16);
            _networkSettings.Verify(x => x.GetMobileNetworkDownloadAllowed(), Times.AtLeastOnce);
            _networkSettings.Verify(x => x.GetPushNotificationsAllowed(), Times.AtLeastOnce);
        }

        [Test]
        public async Task OnSelectedItem_ShouldCrashAppUsingDedicatedButton()
        {
            // Arrange
            _developerPermission.Setup(x => x.IsBmmDeveloper()).Returns(true);

            var settingsViewModel = CreateSettingsViewModel();
            await settingsViewModel.Initialize();

            // Act & Assert
            Assert.Throws<Exception>(() => settingsViewModel.ListItems.OfType<SelectableListItem>().FirstOrDefault(x => x.Title == "Crash the app")?.OnSelected.Execute());
        }

        [Test]
        public async Task BuildSettingsSection_ShouldSetMobileNetworkDownloadAllowed()
        {
            // Arrange
            var settingsViewModel = CreateSettingsViewModel();
            settingsViewModel.ShouldAlwaysRaiseInpcOnUserInterfaceThread(false);
            await settingsViewModel.Initialize();

            // Act
            var item = settingsViewModel
                .ListItems
                .First(s => s.Title == Translations.SettingsViewModel_OptionDownloadMobileNetworkHeader);

            ((CheckboxListItemPO)item).ShouldAlwaysRaiseInpcOnUserInterfaceThread(false);
            ((CheckboxListItemPO)item).IsChecked = false;

            // Assert
            _settingsStorage.Verify(x => x.SetMobileNetworkDownloadAllowed(It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task BuildSettingsSection_ShouldSetSendNotificationStatus()
        {
            // Arrange
            var settingsViewModel = CreateSettingsViewModel();
            settingsViewModel.ShouldAlwaysRaiseInpcOnUserInterfaceThread(false);
            await settingsViewModel.Initialize();

            // Act
            var item = settingsViewModel
                .ListItems
                .First(s => s.Title == Translations.SettingsViewModel_OptionPushNotifications);

            ((CheckboxListItemPO)item).ShouldAlwaysRaiseInpcOnUserInterfaceThread(false);
            ((CheckboxListItemPO)item).IsChecked = true;

            // Assert
            _changeNotificationSettingStateAction.Verify(x => x.ExecuteGuarded(false), Times.Once);
        }

        [Test]
        public async Task BuildAboutSection_ShouldShowInfoAfterClickOption()
        {
            // Arrange
            var settingsViewModel = CreateSettingsViewModel();
            settingsViewModel.ShouldAlwaysRaiseInpcOnUserInterfaceThread(false);
            settingsViewModel.TextSource = TextResource.Object;
            await settingsViewModel.Initialize();

            // Act
            var items = settingsViewModel
                .ListItems
                .First(s => s.Title == Translations.SettingsViewModel_OptionAppVersionHeader);

            ((SelectableListItem)items)?.OnSelected.Execute();

            // Assert
            _userDialogs.Verify(x => x.AlertAsync(It.IsAny<AlertConfig>(), null), Times.Once);
        }
    }
}