using System.Collections.Generic;
using Android.Util;
using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Security;
using BMM.Core.Utils;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using AppCenterLog = Com.Microsoft.Appcenter.Utils.AppCenterLog;
using Exception = System.Exception;

namespace BMM.UI.Droid.Application.Implementations
{
    public class AndroidLogger: ILogger
    {
        private readonly IUserStorage _userStorage;
        private readonly IConnection _connection;

        public AndroidLogger(IUserStorage userStorage, IConnection connection)
        {
            _userStorage = userStorage;
            _connection = connection;
        }
        
        public void Debug(string tag, string message)
        {
            Log.Debug(tag, message);
            AppCenterLog.Debug(tag, message);
        }

        public void Info(string tag, string message)
        {
            Log.Info(tag, message);
            AppCenterLog.Info(tag, message);
        }

        public void Warn(string tag, string message)
        {
            Log.Warn(tag, message);
            AppCenterLog.Warn(tag, message);
        }

        public void Error(string tag, string message)
        {
            Log.Error(tag, message);
            var dic = new Dictionary<string, string>
            {
                {"Tag", tag},
                {"Message", message}
            };
            
            AddAnalyticsId(dic);
            AddConnectionType(dic);
            
            Crashes.TrackError(new ErrorWithoutException(tag + " - " + message), dic);
            AppCenterLog.Error(tag, message);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Error without exception", dic);
        }

        public void Error(string tag, string message, Exception exception, bool presentedToUser)
        {
            var throwableException = Throwable.FromException(exception);
            Log.Error(tag, throwableException, message);
            
            var parameters = new Dictionary<string, string>
            {
                { "Tag", tag },
                { "Message", message },
                { "PresentedToUser", presentedToUser.ToString() }
            };
            
            AddAnalyticsId(parameters);
            AddConnectionType(parameters);
            
            Crashes.TrackError(throwableException, parameters);
            AppCenterLog.Error(tag, message, throwableException);
                       
            parameters.Add("StackTrace", exception.StackTrace);
            parameters.Add("Exception", exception.ToString());
            
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Error with exception", parameters);
        }
        
        private void AddConnectionType(Dictionary<string, string> parameters)
        {
            parameters.Add(AnalyticsConstants.ConnectionParameterName, AnalyticsUtils.GetConnectionType(_connection));
        }
        
        private void AddAnalyticsId(IDictionary<string, string> dic)
        {
            var user = _userStorage.GetUser();
            
            if (user != null)
                dic.Add(nameof(user.AnalyticsId), user.AnalyticsId);
        }
    }
}