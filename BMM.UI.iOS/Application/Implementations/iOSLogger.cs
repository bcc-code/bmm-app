﻿using System;
using System.Collections.Generic;
using BMM.Api.Framework;
using BMM.Core.Constants;
using BMM.Core.Implementations.Exceptions;
using BMM.Core.Implementations.Security;
using BMM.Core.Utils;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;

namespace BMM.UI.iOS.Implementations
{
    public class IosLogger : ILogger
    {
        private readonly IUserStorage _userStorage;
        private readonly IConnection _connection;

        public IosLogger(IUserStorage userStorage, IConnection connection)
        {
            _userStorage = userStorage;
            _connection = connection;
        }
        
        private void WriteToConsole(string tag, string level, string message)
        {
            Console.WriteLine(tag + ":" + level + ":" + message);
        }

        public void Debug(string tag, string message)
        {
            WriteToConsole(tag, "Debug", message);
            AppCenterLog.Debug(tag, message);
        }

        public void Info(string tag, string message)
        {
            WriteToConsole(tag, "Info", message);
            AppCenterLog.Info(tag, message);
        }

        public void Warn(string tag, string message)
        {
            WriteToConsole(tag, "Warn", message);
            AppCenterLog.Warn(tag, message);
        }

        public void Error(string tag, string message)
        {
            WriteToConsole(tag, "Error", message);
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
            WriteToConsole(tag, "Error", message + " Exception: " + exception.Message);

            var parameters = new Dictionary<string, string>
            {
                { "Tag", tag },
                { "Message", message },
                { "PresentedToUser", presentedToUser.ToString() }
            };

            AddAnalyticsId(parameters);
            AddConnectionType(parameters);

            Crashes.TrackError(exception, parameters); 
            AppCenterLog.Error(tag, message, exception);
            
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