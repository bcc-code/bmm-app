using System;
using System.Collections.Generic;
using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Security;
using BMM.Core.Utils;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;

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

        protected BaseLogger(
            IConnection connection,
            IUserStorage userStorage)
        {
            _connection = connection;
            _userStorage = userStorage;
        }
        
        public void Debug(string tag, string message)
        {
            AppCenterLog.Debug(tag, message);
        }

        public void Info(string tag, string message)
        {
            AppCenterLog.Info(tag, message);
        }

        public void Warn(string tag, string message)
        {
            var parameters = InitializeDictionaryWithBasicParameters(tag, message);
            AppCenterLog.Warn(tag, message);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Warning", parameters);
        }

        public virtual void Error(string tag, string message)
        {
            var parameters = InitializeDictionaryWithBasicParameters(tag, message);
            Crashes.TrackError(new ErrorWithoutException(tag + " - " + message), parameters);
            AppCenterLog.Error(tag, message);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Error without exception", parameters);
        }

        public virtual void Error(string tag, string message, Exception exception, bool presentedToUser = false)
        {
            var parameters = InitializeDictionaryWithBasicParameters(tag, message, presentedToUser);
            
            Crashes.TrackError(exception, parameters);
            AppCenterLog.Error(tag, message, exception);
                       
            parameters.Add(StackTrackParameterName, exception.StackTrace);
            parameters.Add(ExceptionParameterName, exception.ToString());
            
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Error with exception", parameters);
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