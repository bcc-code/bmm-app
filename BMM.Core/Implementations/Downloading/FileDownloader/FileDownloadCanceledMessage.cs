using System;
using MvvmCross.Plugin.Messenger;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public class FileDownloadCanceledMessage : MvxMessage
    {
        public Exception Exception { get; }

        public FileDownloadCanceledMessage(object sender, Exception exception) : base(sender)
        {
            Exception = exception;
        }
    }
}