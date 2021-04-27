using System;
using System.Collections.Generic;
using BMM.Api.Framework;
using BMM.Core.Implementations.Exceptions;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;

namespace BMM.UI.iOS.Implementations
{
    public class IosLogger : ILogger
    {
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
            var dic = new Dictionary<string, string> {{"Tag", tag}, {"Message", message}};
            Crashes.TrackError(new ErrorWithoutException(tag + " - " + message), dic);
            AppCenterLog.Error(tag, message);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Error without exception", dic);
        }

        public void Error(string tag, string message, Exception exception)
        {
            WriteToConsole(tag, "Error", message + " Exception: " + exception.Message);
            Crashes.TrackError(exception, new Dictionary<string, string> {{"Tag", tag}, {"Message", message}});
            AppCenterLog.Error(tag, message, exception);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Error with exception",
                new Dictionary<string, string> {{"Tag", tag}, {"Message", message}, {"Exception", exception.ToString()}, {"StackTrace", exception.StackTrace}});
        }
    }
}