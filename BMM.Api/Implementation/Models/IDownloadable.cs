using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public interface IDownloadable
    {
        int Id { get; }

        IEnumerable<string> Tags { get; }

        string Url { get; }
    }
}