using System;

namespace BMM.Core.Implementations.PostLoginActions
{
    public class CouldNotCreateUserException : Exception
    {
        public CouldNotCreateUserException(string message) : base(message)
        { }
    }
}