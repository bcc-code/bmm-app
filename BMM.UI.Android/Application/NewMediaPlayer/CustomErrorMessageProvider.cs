using Android.Support.V4.Media.Session;
using Android.Util;
using BMM.Api.Framework;
using BMM.Core.Implementations.Analytics;
using BMM.UI.Droid.Application.Helpers;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Mediacodec;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Object = Java.Lang.Object;

namespace BMM.UI.Droid.Application.NewMediaPlayer
{
    public class CustomErrorMessageProvider : Object, IErrorMessageProvider
    {
        public const int ErrorInternetProblems = 100;
        public const int SslProblemsBecauseOfOldAndroid = 101;

        private readonly Func<IExoPlayer> _playerFunc;
        private readonly ISdkVersionHelper _sdkVersionHelper;
        private readonly ILogger _logger;

        public CustomErrorMessageProvider(Func<IExoPlayer> playerFunc, ISdkVersionHelper sdkVersionHelper, ILogger logger)
        {
            _playerFunc = playerFunc;
            _sdkVersionHelper = sdkVersionHelper;
            _logger = logger;
        }

        public Pair GetErrorMessage(Object exceptionObject)
        {
            return GetErrorMessage((ExoPlaybackException)_playerFunc().PlayerError);
        }

        public Pair GetErrorMessage(ExoPlaybackException exception)
        {
            if (exception.Type == ExoPlaybackException.TypeSource)
            {
                if (exception.SourceException is IHttpDataSource.HttpDataSourceException internetProblems)
                {
                    if (internetProblems.Cause is Javax.Net.Ssl.SSLHandshakeException)
                    {
                        // ExoPlayer uses an outdated mechanism to do the SSL connection relying on TLS 1.0 when using old versions of Android.
                        // Unfortunately BTV requires TLS 1.2 or newer for SSL, therefore causing an error during the SSL Handshake.

                        if (_sdkVersionHelper.HasProblemsWithSslHandshakes)
                        {
                            return new Pair(SslProblemsBecauseOfOldAndroid, "");
                        }
                        else
                        {
                            // The assumption is that this error only happens on older versions
                            _logger.Error("CustomErrorMessageProvider", "Has problems with SSL Handshakes even though being on a newer version", exception.SourceException);
                        }
                    }
                    return new Pair(ErrorInternetProblems, "Internet problems: " + internetProblems);
                }

                if (exception.SourceException is FileDataSource.FileDataSourceException fileProblem)
                {
                    // trying to play a local file but the file has been deleted
                    MvvmCross.Mvx.IoCProvider.Resolve<IAnalytics>()
                        .LogEvent("Playback error because the file was deleted", new Dictionary<string, object> {{"message", fileProblem.Message}});
                }

                return new Pair(PlaybackStateCompat.ErrorCodeNotSupported, "Problem with the source: " + exception.SourceException);
            }
            else if (exception.Type == ExoPlaybackException.TypeRenderer)
            {
                if (exception.RendererException is MediaCodecRenderer.DecoderInitializationException)
                {
                    return new Pair(PlaybackStateCompat.ErrorCodeNotSupported, "Problem with decoder");
                }

                return new Pair(PlaybackStateCompat.ErrorCodeUnknownError, "Problem with the renderer");
            }
            else
            {
                return new Pair(PlaybackStateCompat.ErrorCodeUnknownError, "Unexpected error: " + exception.UnexpectedException);
            }
        }
    }
}