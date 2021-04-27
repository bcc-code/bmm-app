namespace BMM.Core.Implementations.Analytics
{
    /* FirebaseEventAdjuster was created to adjust already existed events which were meant to be logged in AppCenter originally to meet the requirements of FirebaseAnalytics too. */
    public class FirebaseEventAdjuster
    {
        /// <summary>
        /// Adjusts event name to be up to 40 characters long, to only contain alphanumeric characters and underscores("_"), and to start with an alphabetic character.
        /// </summary>
        public static string AdjustEventNameForFirebase(string eventName)
        {
            string preparedString = eventName?.Replace(" ", "_").ToLower();

            if (preparedString?.Length > 40)
            {
                preparedString = preparedString.Remove(40);
            }

            return preparedString;
        }
    }
}
