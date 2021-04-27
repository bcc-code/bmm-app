using System.Collections.Generic;
using Android.Util;
using BMM.Api.Framework;
using BMM.Core.Implementations.Exceptions;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using AppCenterLog = Com.Microsoft.Appcenter.Utils.AppCenterLog;
using Exception = System.Exception;

namespace BMM.UI.Droid.Application.Implementations
{
    public class AndroidLogger: ILogger
    {
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
            var dic = new Dictionary<string, string> {{"Tag", tag}, {"Message", message}};
            Crashes.TrackError(new ErrorWithoutException(tag + " - " + message), dic);
            AppCenterLog.Error(tag, message);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Error without exception", dic);
        }

        public void Error(string tag, string message, Exception exception)
        {
            var throwableException = Throwable.FromException(exception);

            Log.Error(tag, throwableException, message);
            Crashes.TrackError(throwableException, new Dictionary<string, string> {{"Tag", tag}, {"Message", message}});
            AppCenterLog.Error(tag, message, throwableException);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Error with exception",
                new Dictionary<string, string> {{"Tag", tag}, {"Message", message}, {"Exception", exception.ToString()}, {"StackTrace", exception.StackTrace}});
        }
    }
}