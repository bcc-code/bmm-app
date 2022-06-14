using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Implementation.Models;
using BMM.Core.GuardedActions.App.Interfaces;
using BMM.Core.Helpers;
using BMM.Core.Helpers.PresentationHints;
using BMM.Core.Implementations;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Caching;
using BMM.Core.Implementations.Device;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.Player.Interfaces;
using BMM.Core.Implementations.Security;
using BMM.Core.Implementations.Security.Oidc;
using BMM.Core.Implementations.Security.Oidc.Interfaces;
using BMM.Core.Messages;
using BMM.Core.Models;
using BMM.Core.Models.Storage;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.Core.ViewModels;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace BMM.Core
{
    public interface IAppNavigator
    {
        void NavigateAtAppStart();

        Task NavigateAfterLoggedIn();

        Task NavigateToLogin(bool isInitialLogin);
    }

    public class AppNavigator : IAppNavigator
    {
        private const int TimeToDetectIfAppOpenedFromDeepLinkInMs = 500;
        private readonly IDeviceInfo _deviceInfo;
        private readonly IMvxNavigationService _navigationService;
        private readonly IMvxMessenger _messenger;
        private readonly IOidcAuthService _oidcAuthService;
        private readonly IStopwatchManager _stopwatchManager;
        private readonly IAnalytics _analytics;
        private readonly IUserStorage _userStorage;
        private readonly IAppContentLogger _appContentLogger;
        private readonly ILanguagesLogger _languagesLogger;
        private readonly ICache _cache;
        private readonly IMediaPlayer _mediaPlayer;
        private readonly IRememberedQueueInfoService _rememberedQueueInfoService;
        private readonly SupportVersionChecker _supportVersionChecker;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IOnStartAction _onStartAction;
        private Stopwatch _stopwatch;

        public AppNavigator(
            IDeviceInfo deviceInfo,
            IMvxNavigationService navigationService,
            IMvxMessenger messenger,
            IOidcAuthService oidcAuthService,
            IStopwatchManager stopwatchManager,
            IAnalytics analytics,
            IUserStorage userStorage,
            IAppContentLogger appContentLogger,
            ILanguagesLogger languagesLogger,
            ICache cache,
            IMediaPlayer mediaPlayer,
            IRememberedQueueInfoService rememberedQueueInfoService,
            SupportVersionChecker supportVersionChecker,
            IAccessTokenProvider accessTokenProvider,
            IOnStartAction onStartAction)
        {
            _deviceInfo = deviceInfo;
            _navigationService = navigationService;
            _messenger = messenger;
            _oidcAuthService = oidcAuthService;
            _stopwatchManager = stopwatchManager;
            _analytics = analytics;
            _userStorage = userStorage;
            _appContentLogger = appContentLogger;
            _languagesLogger = languagesLogger;
            _cache = cache;
            _mediaPlayer = mediaPlayer;
            _rememberedQueueInfoService = rememberedQueueInfoService;
            _supportVersionChecker = supportVersionChecker;
            _accessTokenProvider = accessTokenProvider;
            _onStartAction = onStartAction;
        }

        /// <summary>
        /// We can't make this async since iOS freezes.
        /// This is an issues of MVVMCross which creates a deadlock when something is awaited in the first view models <see cref="IMvxViewModel.Initialize"/>
        /// See https://github.com/MvvmCross/MvvmCross/pull/3222
        /// </summary>
        public void NavigateAtAppStart()
        {
            if(!_supportVersionChecker.IsCurrentDeviceVersionSupported())
            {
                NavigateToSupportEndedPageWithMessage(SupportEndedMessage.DeviceSupportEnded);
                return;
            }

            if (!_supportVersionChecker.IsCurrentAppVersionSupported())
            {
                NavigateToSupportEndedPageWithMessage(SupportEndedMessage.ApplicationVersionSupportEnded);
                return;
            }

            var user = _userStorage.GetUser();

            if (user != null)
            {
                _analytics.LogEvent("User started the app",
                    new Dictionary<string, object>
                    {
                        {"secondsSinceAppStart", _stopwatchManager.GetStopwatch(StopwatchType.AppStart).Elapsed.TotalSeconds}
                    });

                var exceptionHandler = Mvx.IoCProvider.Resolve<IExceptionHandler>();
                exceptionHandler.FireAndForget(() => _appContentLogger.LogAppContent("Log app content at AppStart"));
                exceptionHandler.FireAndForget(() => _languagesLogger.LogAppAndContentLanguages("Log app and content languages at AppStart"));
            }

            _stopwatch = _stopwatchManager.StartAndGetStopwatch(StopwatchType.NavigateAtAppStart);

            if (CallSynchronous(IsAuthenticated))
            {
                _accessTokenProvider.Initialize();
                if (IsOffline())
                {
                    Log("Logged in but offline");
                }
                else
                {
                    _messenger.Publish(new LoggedInOnlineMessage(this));
                    Log("Logged in and online");
                }

                NavigateAfterLoggedIn()
                    .ContinueWith(_ => RestoreMediaQueue())
                    .ContinueWith(_ => _onStartAction.ExecuteGuarded());
            }
            else
            {
                Log("Not logged in");
                NavigateToLogin(true);
            }
        }

        private async Task RestoreMediaQueue()
        {
            try
            {
                await _accessTokenProvider.UpdateAccessTokenIfNeeded();

                if (_deviceInfo.IsIos)
                    await Task.Delay(TimeToDetectIfAppOpenedFromDeepLinkInMs);
                
                if (_rememberedQueueInfoService.PreventRecoveringQueue)
                    return;

                bool queueSaved = await _cache.ContainsKeys(StorageKeys.RememberedQueue, StorageKeys.CurrentTrackPosition);

                if (!queueSaved)
                    return;

                var rememberedQueue = await _cache.GetObject<IList<Track>>(StorageKeys.RememberedQueue);
                var currentTrackPosition = await _cache.GetObject<CurrentTrackPositionStorage>(StorageKeys.CurrentTrackPosition);

                bool canRestoreMediaQueue = rememberedQueue != null
                                            && currentTrackPosition != null;

                if (!canRestoreMediaQueue)
                    return;

                var currentTrack = rememberedQueue
                    .FirstOrDefault(t => t.Id == currentTrackPosition.CurrentTrackId);

                if (currentTrack == null)
                    return;

                await _mediaPlayer.RecoverQueue(new List<IMediaTrack>(rememberedQueue),
                    currentTrack,
                    currentTrackPosition.LastPosition);
            }
            catch (InternetProblemsException)
            {
                //ignore
            }
            catch (Exception e)
            {
                await _cache.Invalidate(StorageKeys.RememberedQueue);
                await _cache.Invalidate(StorageKeys.CurrentTrackPosition);
                Log($"Error during {nameof(RestoreMediaQueue)}. EX: {e.Message}. Remembered queue cleared");
            }
            finally
            {
                _rememberedQueueInfoService.NotifyAfterRecoveringQueue();
            }
        }

        private void Log(string message)
        {
            _stopwatch.Stop();
            var sinceAppStart = _stopwatchManager.GetStopwatch(StopwatchType.AppStart);
            _analytics.LogEvent("AppNavigator - " + message,
                new Dictionary<string, object> {{"secondsElapsed", _stopwatch.Elapsed.TotalSeconds}, {"secondsSinceAppStart", sinceAppStart.Elapsed.TotalSeconds}});
        }

        public async Task NavigateAfterLoggedIn()
        {
            if (_deviceInfo.IsIos)
                await _navigationService.NavigateToNewRoot<MenuViewModel>();
            else
                await _navigationService.NavigateToNewRoot<ExploreNewestViewModel>();
            _messenger.Publish(new LoggedInMessage(this));
        }

        public async Task NavigateToLogin(bool isInitialLogin)
        {
            await _navigationService.ChangePresentation(new ClearAllNavBackStackHint());
            await _navigationService.Navigate<OidcLoginViewModel, OidcLoginParameters>(new OidcLoginParameters {IsInitialLogin = isInitialLogin});
        }

        private Task<bool> NavigateToSupportEndedPageWithMessage(SupportEndedMessage message)
        {
            return _navigationService.Navigate<SupportEndedViewModel, SupportEndedParameters>
                (new SupportEndedParameters{ SupportEndedMessage = message });
        }

        private Task<bool> IsAuthenticated()
        {
            return _oidcAuthService.IsAuthenticated();
        }

        private bool IsOffline()
        {
            var result = Mvx.IoCProvider.Resolve<IConnection>().GetStatus();
            return result == ConnectionStatus.Offline;
        }

        /// <summary>
        /// iOS requires AppStart to run synchronous. Therefore we convert it to a synchronous call.
        /// </summary>
        private bool CallSynchronous(Func<Task<bool>> action)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(async () => {
                try
                {
                    var isAuth = await action.Invoke();

                    tcs.SetResult(isAuth);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }).ConfigureAwait(false);

            if (tcs.Task.Exception != null)
            {
                throw new AggregateException(tcs.Task.Exception);
            }

            return tcs.Task.Result;
        }
    }
}
