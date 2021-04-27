using System;

namespace BMM.Api.Framework.Exceptions
{
    /// <summary>
    /// No Internet available and we didn't even try to send the request.
    /// </summary>
    public class NoInternetException : Exception
    {
    }
}
