using System;

namespace BMM.Core.Implementations.Exceptions
{
    public class ErrorWithoutException : Exception
    {
        public ErrorWithoutException(string message) : base(message)
        {
        }
    }
}
