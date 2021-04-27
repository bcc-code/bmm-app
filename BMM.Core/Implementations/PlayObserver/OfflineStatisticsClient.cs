using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private const int UploadThresholdInMinutes = 5;

        private readonly ITrackPlayedStorage _trackPlayedStorage;
        private readonly IConnection _connection;
        private readonly IExceptionHandler _exceptionHandler;
        private bool _requestIsRunning;
        private DateTimeOffset? _lastPushed;

        public OfflineStatisticsClient(IRequestHandler handler, ApiBaseUri baseUri, ITrackPlayedStorage trackPlayedStorage, IConnection connection,
            IExceptionHandler exceptionHandler, ILogger logger) : base(handler, baseUri, logger)
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

            if (!_lastPushed.HasValue || LastPushedExceedsThreshold() && !_requestIsRunning)
            {
                _requestIsRunning = true;
                _exceptionHandler.FireAndForgetWithoutUserMessages(async () =>
                {
                    try
                    {
                        var allEvents = await _trackPlayedStorage.GetExistingEvents();
                        await base.PostTrackPlayedEvent(allEvents);
                        await _trackPlayedStorage.DeleteEvents(allEvents);
                        _lastPushed = DateTimeOffset.Now;
                    }
                    finally
                    {
                        _requestIsRunning = false;
                    }
                });
            }
        }

        private bool LastPushedExceedsThreshold()
        {
            return _lastPushed.HasValue && _lastPushed.Value.AddMinutes(UploadThresholdInMinutes) < DateTimeOffset.Now;
        }
    }
}