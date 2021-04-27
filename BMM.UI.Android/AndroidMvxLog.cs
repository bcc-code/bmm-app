using System;
using System.Collections.Generic;
using MvvmCross.Logging;
using AndroidLog = Android.Util.Log;

namespace BMM.UI.Droid
{
    [Obsolete("I'm not sure if we even need this")]
    public class AndroidMvxLog : IMvxLog
    {
        public bool Log(MvxLogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            try
            {
                var parts = new List<string>
                {
                    logLevel.ToString(),
                    string.Format(messageFunc(), formatParameters)
                };
                if (exception != null)
                {
                    parts.Add(exception.ToString());
                }

                var message = string.Join(':', parts);

                switch (logLevel)
                {
                    case MvxLogLevel.Trace:
                    case MvxLogLevel.Debug:
                        AndroidLog.Debug("", message);
                        break;

                    case MvxLogLevel.Info:
                        AndroidLog.Info("", message);
                        break;
                    case MvxLogLevel.Warn:
                        AndroidLog.Warn("", message);
                        break;
                    default:
                        AndroidLog.Error("", message);
                        break;
                }

                return true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Exception during Logging of {0} {1}", logLevel, messageFunc());
                return false;
            }
        }

        public bool IsLogLevelEnabled(MvxLogLevel logLevel)
        {
            return true;
        }
    }
}