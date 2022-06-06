using System;

namespace BMM.Api.Framework
{
    public interface ILogger
    {
        void Debug(string tag, string message);
        void Info(string tag, string message);
        void Warn(string tag, string message);
        void Error(string tag, string message);
        void Error(string tag, string message, Exception exception, bool wasErrorMessagePresentedToUser = false);
    }
}