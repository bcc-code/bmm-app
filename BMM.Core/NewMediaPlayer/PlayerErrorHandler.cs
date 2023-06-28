using System;
using System.Collections.Generic;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Implementations.UI;
using BMM.Core.Translation;
using BMM.Core.Utils;
using MvvmCross.Localization;

namespace BMM.Core.NewMediaPlayer
{
    public class PlayerErrorHandler : IPlayerErrorHandler
    {
        private const string ErrorPlayerStopped = Translations.MediaPlayer_ErrorPlayerStopped;
        private const string ErrorPlayerStart = Translations.MediaPlayer_ErrorPlayerStart;
        private const string ErrorPlayerStoppedGeneric = Translations.MediaPlayer_ErrorPlayerStoppedGeneric;
        private const string ErrorPlayerLiveRadioStopped = Translations.MediaPlayer_ErrorPlayerLiveRadioStopped;
        private const string ErrorPlayerLiveRadioTooEarly = Translations.MediaPlayer_ErrorPlayerLiveRadioTooEarly;

        private const int MaximumParameterLength = 120;

        private readonly ILogger _logger;
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IAnalytics _analytics;
        private readonly IBMMLanguageBinder _bmmLanguageBinder;

        public PlayerErrorHandler(
            ILogger logger,
            IToastDisplayer toastDisplayer,
            IAnalytics analytics,
            IBMMLanguageBinder bmmLanguageBinder)
        {
            _logger = logger;
            _toastDisplayer = toastDisplayer;
            _analytics = analytics;
            _bmmLanguageBinder = bmmLanguageBinder;
        }

        public void InternetProblems(string technicalMessage)
        {
            // iOS provides very good human readable messages but in Android we unfortunately have to generalize.
            InternetProblems(technicalMessage, _bmmLanguageBinder[Translations.Global_InternetConnectionOffline]);
        }

        public void InternetProblems(string technicalMessage, string localizedUserReadableMessage)
        {
            var message = _bmmLanguageBinder.GetText(ErrorPlayerStopped);
            _toastDisplayer.Error($"{message} {localizedUserReadableMessage}");
            _logger.Warn("MediaPlayer", $"The player has stopped because of internet problems. {technicalMessage}");
        }

        public void LiveRadioStopped()
        {
            _toastDisplayer.Error(_bmmLanguageBinder.GetText(ErrorPlayerLiveRadioStopped));
        }

        public void LiveRadioTooEarly()
        {
            _toastDisplayer.Error(_bmmLanguageBinder.GetText(ErrorPlayerLiveRadioTooEarly));
        }

        public void StartError(Exception exception)
        {
            _toastDisplayer.Error(_bmmLanguageBinder.GetText(ErrorPlayerStart));
            _logger.Error("MediaPlayer", "Unable to start playback", exception);
        }

        public void PlaybackError(string technicalMessage, ITrackModel currentlyPlayedTrack, string userReadableMessage = null)
        {
            // Android does not provide a usable and localized error message. Therefore we have to use a generic message instead.
            var localizedErrorMessage = userReadableMessage != null
                ? $"{_bmmLanguageBinder.GetText(ErrorPlayerStopped)} {userReadableMessage}"
                : _bmmLanguageBinder.GetText(ErrorPlayerStoppedGeneric);

            _toastDisplayer.Error(localizedErrorMessage);
            LogPlaybackError(technicalMessage, currentlyPlayedTrack);
        }

        private void LogPlaybackError(string technicalMessage, ITrackModel currentlyPlayedTrack)
        {
            var errorMessage = $"The player has stopped. {technicalMessage}";
            _logger.Error("MediaPlayer", errorMessage);

            var trackIsDownloaded = currentlyPlayedTrack == null ? "null" : (currentlyPlayedTrack.LocalPath != null).ToString();
            var idString = currentlyPlayedTrack?.Id.ToString() ?? "null";
            var isLivePlaybackString = currentlyPlayedTrack?.IsLivePlayback.ToString() ?? "null";

            var url = currentlyPlayedTrack?.Url ?? "null";
            var localPath = currentlyPlayedTrack?.LocalPath ?? "null";
            var pathToFile = trackIsDownloaded == "False" ? url : localPath;

            var parameters = new Dictionary<string, object>
            {
                { "trackId", idString },
                { "trackIsDownloaded", trackIsDownloaded },
                { "isLiveTrack", isLivePlaybackString },
                { "pathToFile", pathToFile },
            };

            parameters.AddParameter("technicalMessage", technicalMessage, MaximumParameterLength);

            _analytics.LogEvent("A playback error has occured.", parameters);
        }
    }
}