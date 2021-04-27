using System.Collections.Generic;

namespace BMM.Core.Implementations.Analytics
{
    public interface IAnalytics
    {
        /// <summary>
        /// Logs event in AppCenter and FirebaseAnalytics.
        /// </summary>
        /// <param name="eventName">Name can be up to 40 characters long, may only contain alphanumeric characters and underscores("_"), and must start with an alphabetic character.</param>
        void LogEvent(string eventName);

        /// <summary>
        /// Logs event in AppCenter and FirebaseAnalytics.
        /// </summary>
        /// <param name="eventName">Name can be up to 40 characters long, may only contain alphanumeric characters and underscores("_"), and must start with an alphabetic character.</param>
        /// <param name="parameters">Param values can be up to 100 characters long. The "firebase_", "google_", and "ga_" prefixes are reserved and should not be used.</param>
        void LogEvent(string eventName, IDictionary<string, object> parameters);
    }
}