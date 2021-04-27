using System;
using System.Collections.Generic;
using MvvmCross.Logging;

namespace BMM.UI.iOS
{
    [Obsolete("I don't think we need this since the default implementation already does the same")]
    public class ConsoleLogger : IMvxLog
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

                Console.WriteLine(string.Join(':', parts));
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