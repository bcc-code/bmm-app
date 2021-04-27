using System;
using BMM.Api.Abstraction;

namespace BMM.Core.NewMediaPlayer
{
    public interface IPlayerErrorHandler
    {
        void PlaybackError(string technicalMessage, ITrackModel currentlyPlayedTrack, string userReadableMessage = null);

        void StartError(Exception exception);

        void InternetProblems(string technicalMessage);
        void InternetProblems(string technicalMessage, string localizedUserReadableMessage);

        void LiveRadioStopped();
        void LiveRadioTooEarly();
    }
}