using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Implementation;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Models;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.PlayObserver.Storage;

namespace BMM.Core.Implementations.PlayObserver
{
    public class OfflineStatisticsClient : StatisticsClient
    {
        private readonly ITrackPlayedStorage _trackPlayedStorage;
        private readonly IConnection _connection;
        private readonly IExceptionHandler _exceptionHandler;
        private bool _requestIsRunning;

        public OfflineStatisticsClient(IRequestHandler handler,
            ApiBaseUri baseUri,
            ITrackPlayedStorage trackPlayedStorage,
            IConnection connection,
            IExceptionHandler exceptionHandler,
            ILogger logger) : base(handler, baseUri, logger)
        {
            _trackPlayedStorage = trackPlayedStorage;
            _connection = connection;
            _exceptionHandler = exceptionHandler;
        }

        public override async Task PostTrackPlayedEvent(IEnumerable<TrackPlayedEvent> trackPlayedEvents)
        {
            await _trackPlayedStorage.Add(trackPlayedEvents);
            if (_connection.GetStatus() == ConnectionStatus.Offline)
                return;

            if (!_requestIsRunning)
            {
                _requestIsRunning = true;
                _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
                {
                    try
                    {
                        var allEvents = _trackPlayedStorage.GetUnsentTrackPlayedEvents();
                        await base.PostTrackPlayedEvent(allEvents);
                        await _trackPlayedStorage.DeleteEvents(allEvents);
                    }
                    finally
                    {
                        _requestIsRunning = false;
                    }
                });
            }
        }
        
        public override async Task PostListeningEvents(IList<ListeningEvent> listeningEvents)
        {
            try
            {
                var allEvents = _trackPlayedStorage.GetUnsentListeningEvents();

                foreach (var listeningEvent in listeningEvents)
                    allEvents.Add(listeningEvent);
                
                await base.PostListeningEvents(allEvents);
                _trackPlayedStorage.ClearUnsentListeningEvents();
            }
            catch
            {
                await _trackPlayedStorage.AddListeningEvents(listeningEvents);
            }
        }

        public override async Task PostStreakPoints(IList<StreakPointEvent> streakPointEvents)
        {
            try
            {
                var allEvents = _trackPlayedStorage.GetUnsentStreakPointEvents();

                foreach (var streakPointEvent in streakPointEvents)
                    allEvents.Add(streakPointEvent);
                
                await base.PostStreakPoints(allEvents);
                _trackPlayedStorage.ClearUnsentStreakPointsEvents();
            }
            catch
            {
                await _trackPlayedStorage.Add(streakPointEvents);
            }
        }
    }
}