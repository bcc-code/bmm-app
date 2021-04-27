using System.Collections.Generic;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public class PersistedDownloadable : IDownloadable
    {
        public int Id { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Url { get; set; }
    }
}
