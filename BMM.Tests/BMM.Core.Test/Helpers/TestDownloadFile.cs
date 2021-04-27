using System.Collections.Generic;
using System.ComponentModel;
using BMM.Core.Implementations.DownloadManager;

namespace BMM.Core.Test.Helpers
{
    public class TestDownloadFile : IDownloadFile
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Url { get; }

        public IDictionary<string, string> Headers { get; }

        public DownloadFileStatus Status { get; }

        public string StatusDetails { get; }

        public float TotalBytesExpected { get; }

        public float TotalBytesWritten { get; }
    }
}
