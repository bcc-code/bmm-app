using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Notifications;
using BMM.Core.Implementations.Notifications.Data;
using BMM.Core.Implementations.UI;
using BMM.Core.Messages.MediaPlayer;
using BMM.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.PlayObserver
{
    public class AslaksenNotification : IReceiveLocal<WordOfFaithNotification>
    {
        private MvxSubscriptionToken _trackCompletedToken;
        private MvxSubscriptionToken _trackChangedToken;
        private readonly INotificationDisplayer _notificationDisplayer;
        private readonly IBlobCache _localStorage;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IUriOpener _uriOpener;

        public IMvxLanguageBinder TextSource => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "AslaksenNotification");

        private HashSet<int> _notifiedTracksIds;
        private ITrackModel _currentlyPlayingAslaksenTrack;
        private DateTime? _lastNotification;

        public AslaksenNotification(IMvxMessenger messenger, INotificationDisplayer notificationDisplayer, IBlobCache localStorage, IExceptionHandler exceptionHandler, IUriOpener uriOpener)
        {
            _notificationDisplayer = notificationDisplayer;
            _localStorage = localStorage;
            _exceptionHandler = exceptionHandler;
            _uriOpener = uriOpener;
            _trackCompletedToken = messenger.Subscribe<TrackCompletedMessage>(TrackCompleted);
            _trackChangedToken = messenger.Subscribe<CurrentTrackChangedMessage>(TrackChanged);
        }

        private void TrackChanged(CurrentTrackChangedMessage message)
        {
            var currentTrack = message.CurrentTrack;
            if (currentTrack != null && currentTrack.Tags.Contains(AslaksenTeaserViewModel.AsklaksenTagName))
                _currentlyPlayingAslaksenTrack = message.CurrentTrack;
            else
                _currentlyPlayingAslaksenTrack = null;
        }

        private void TrackCompleted(TrackCompletedMessage message)
        {
            if (_currentlyPlayingAslaksenTrack == null)
            {
                return;
            }

            _exceptionHandler.FireAndForgetWithoutUserMessages(() => ShowNotification(_currentlyPlayingAslaksenTrack));
        }

        public async Task ShowNotification(ITrackModel track)
        {
            await InitAsyncIfNeeded();
            if (!_notifiedTracksIds.Contains(track.Id) && IsFirstNotificationOfTheDay())
            {
                var url = "https://aslaksen.bcc.no/quiz/";
                _notificationDisplayer.DisplayNotificationOrPopup(new WordOfFaithNotification
                {
                    Title = TextSource.GetText("Title"), Message = TextSource.GetText("Message"), Url = url, YesText = TextSource.GetText("ButtonYes"),
                    CancelText = TextSource.GetText("ButtonCancel")
                });
                await StoreLastNotificationTime(DateTime.Now); // We don't take norwegian timezone but the timezone of the user
            }

            // Event though we might not have shown the notification, we don't want to show it for this track at a later time either and therefore add it to notified tracks
            await StoreNotifiedTrack(track.Id);
        }

        public bool IsFirstNotificationOfTheDay()
        {
            return _lastNotification?.Date != DateTime.Now.Date;
        }

        private async Task InitAsyncIfNeeded()
        {
            if (_notifiedTracksIds != null)
                return;

            _lastNotification = await _localStorage.GetOrCreateObject<DateTime?>(StorageKeys.LastAslaksenNotification, () => null);
            _notifiedTracksIds = await _localStorage.GetOrCreateObject(StorageKeys.NotifiedAslaksenTracks, () => new HashSet<int>());
        }

        private async Task StoreLastNotificationTime(DateTime dateTime)
        {
            await _localStorage.InsertObject(StorageKeys.LastAslaksenNotification, dateTime);
            _lastNotification = dateTime;
        }

        private async Task StoreNotifiedTrack(int trackId)
        {
            await InitAsyncIfNeeded();
            var oldCount = _notifiedTracksIds.Count;
            _notifiedTracksIds.Add(trackId);
            if (oldCount != _notifiedTracksIds.Count)
            {
                await _localStorage.InsertObject(StorageKeys.NotifiedAslaksenTracks, _notifiedTracksIds);
            }
        }

        public void UserClickedLocalNotification(WordOfFaithNotification notification)
        {
            _uriOpener.OpenUri(new Uri(notification.Url));
        }
    }
}