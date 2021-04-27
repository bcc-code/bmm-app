using System;

namespace BMM.Api.Framework.HTTP
{
    public class DeserializationException : Exception
    {
        public DeserializationException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}