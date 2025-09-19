using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.Implementations.Security;
using BMM.Core.Utils;

namespace BMM.Core.Implementations.Logger
{
    public abstract class BaseLogger : ILogger
    {
        private const string TagParameterName = "Tag";
        private const string MessageParameterName = "Message";
        private const string PresentedToUserParameterName = "PresentedToUser";
        private const string StackTrackParameterName = "StackTrack";
        private const string ExceptionParameterName = "Exception";
        
        private readonly IConnection _connection;
        private readonly IUserStorage _userStorage;

        protected IUserStorage UserStorage => _userStorage;

        protected BaseLogger(
            IConnection connection,
            IUserStorage userStorage)
        {
            _connection = connection;
            _userStorage = userStorage;
        }

        public void Debug(string tag, string message)
        {
            Console.WriteLine($"DEBUG - {tag} - {message}");
        }

        public void Info(string tag, string message)
        {
            Console.WriteLine($"INFO - {tag} - {message}");
        }

        public abstract void TrackEvent(string message, IDictionary<string, string> properties);

        public void Warn(string tag, string message)
        {
            var parameters = InitializeDictionaryWithBasicParameters(tag, message);
            Console.WriteLine($"WARN - {tag} - {message}");
            TrackEvent("Warning", parameters);
        }

        public virtual void Error(string tag, string message)
        {
            var parameters = InitializeDictionaryWithBasicParameters(tag, message);

            SentrySdk.CaptureMessage(tag + " - " + message,
                GetSentryScope(parameters),
                SentryLevel.Error);
            
            TrackEvent("Error without exception", parameters);
        }

        public virtual void Error(string tag, string message, Exception exception, bool presentedToUser = false)
        {
            var parameters = InitializeDictionaryWithBasicParameters(tag, message, presentedToUser);
            
            SentrySdk.CaptureException(exception, GetSentryScope(parameters));
            Console.WriteLine($"ERROR - {tag} - {message} \n \n {exception}");
         
            parameters.Add(StackTrackParameterName, exception.StackTrace);
            parameters.Add(ExceptionParameterName, exception.ToString());
            
           TrackEvent("Error with exception", parameters);
        }

        private static Action<Scope> GetSentryScope(IDictionary<string, string> parameters)
        {
            return scope =>
            {
                scope.SetExtras(parameters.Select(s => new KeyValuePair<string, object>(s.Key, s.Value)));
            };
        }
        
        private void AddConnectionType(IDictionary<string, string> parameters)
        {
            parameters.Add(AnalyticsConstants.ConnectionParameterName, AnalyticsUtils.GetConnectionType(_connection));
        }

        private void AddAnalyticsId(IDictionary<string, string> dic)
        {
            var user = _userStorage.GetUser();
            
            if (user != null)
                dic.Add(nameof(user.AnalyticsId), user.AnalyticsId);
        }
        
        private IDictionary<string, string> InitializeDictionaryWithBasicParameters(string tag, string message, bool? presentedToUser = null)
        {
            var dic = new Dictionary<string, string>
            {
                {TagParameterName, tag},
                {MessageParameterName, message}
            };
            
            if (presentedToUser.HasValue)
                dic.Add(PresentedToUserParameterName, presentedToUser.Value.ToString());
            
            AddAnalyticsId(dic);
            AddConnectionType(dic);
            return dic;
        }
    }
}