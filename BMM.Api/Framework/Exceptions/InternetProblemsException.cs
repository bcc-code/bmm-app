using System;

namespace BMM.Api.Framework.Exceptions
{
    /// <summary>
    /// Is it an exception that is related to the internet connection. Most common case would be that the device is (temporarily) disconnected from the internet.
    /// </summary>
    public class InternetProblemsException : Exception
    {
        public InternetProblemsException(Exception originalException) : base("Unable to perform the request", originalException)
        { }
    }
}
