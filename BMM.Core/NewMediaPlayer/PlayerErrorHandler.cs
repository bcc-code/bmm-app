using System;
using System.Collections.Generic;
using BMM.Api.Abstraction;
using BMM.Api.Framework;
using BMM.Core.Helpers;
using BMM.Core.Implementations.Analytics;
using BMM.Core.Implementations.UI;
using MvvmCross.Localization;

namespace BMM.Core.NewMediaPlayer
{
    public class PlayerErrorHandler : IPlayerErrorHandler
    {
        private const string ErrorPlayerStopped = "ErrorPlayerStopped";
        private const string ErrorPlayerStart = "ErrorPlayerStart";
        private const string ErrorPlayerStoppedGeneric = "ErrorPlayerStoppedGeneric";
        private const string ErrorPlayerLiveRadioStopped = "ErrorPlayerLiveRadioStopped";
        private const string ErrorPlayerLiveRadioTooEarly = "ErrorPlayerLiveRadioTooEarly";

        private readonly ILogger _logger;
        private readonly IToastDisplayer _toastDisplayer;
        private readonly IMvxLanguageBinder _textSource;
        private readonly IAnalytics _analytics;

        public PlayerErrorHandler(ILogger logger, IToastDisplayer toastDisplayer, IAnalytics analytics)
        {
            _logger = logger;
            _toastDisplayer = toastDisplayer;
            _analytics = analytics;

            _textSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "MediaPlayer");
        }

        public void InternetProblems(string technicalMessage)
        {
            MvxLanguageBinder globalTextSource = new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Global");

            // iOS provides very good human readable messages but in Android we unfortunately have to generalize.
            InternetProblems(technicalMessage, globalTextSource.GetText("InternetConnectionOffline"));
        }

        public void InternetProblems(string technicalMessage, string localizedUserReadableMessage)
        {
            // InternetProblems are totally expected and don't need to be logged as an error. Other problems however might point to problems in the app or on the server.

            var message = _textSource.GetText(ErrorPlayerStopped);
            _toastDisplayer.Error($"{message} {localizedUserReadableMessage}");
            _logger.Warn("MediaPlayer", $"The player has stopped because of internet problems. {technicalMessage}");
        }

        public void LiveRadioStopped()
        {
            _toastDisplayer.Error(_textSource.GetText(ErrorPlayerLiveRadioStopped));
        }

        public void LiveRadioTooEarly()
        {
            _toastDisplayer.Error(_textSource.GetText(ErrorPlayerLiveRadioTooEarly));
        }

        public void StartError(Exception exception)
        {
            _toastDisplayer.Error(_textSource.GetText(ErrorPlayerStart));
            _logger.Error("MediaPlayer", "Unable to start playback", exception);
        }

        public void PlaybackError(string technicalMessage, ITrackModel currentlyPlayedTrack, string userReadableMessage = null)
        {
            // Android does not provide a usable and localized error message. Therefore we have to use a generic message instead.
            var localizedErrorMessage = userReadableMessage != null
                ? $"{_textSource.GetText(ErrorPlayerStopped)} {userReadableMessage}"
                : _textSource.GetText(ErrorPlayerStoppedGeneric);

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

            _analytics.LogEvent("A playback error has occured.",
                new Dictionary<string, object>
                {
                    {"technicalMessage", technicalMessage},
                    {"trackId", idString},
                    {"trackIsDownloaded", trackIsDownloaded},
                    {"isLiveTrack", isLivePlaybackString},
                    {"pathToFile", pathToFile},
                });
        }
    }
}