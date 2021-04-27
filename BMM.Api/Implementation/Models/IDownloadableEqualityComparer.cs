using System.Collections.Generic;

namespace BMM.Api.Implementation.Models
{
    public class DownloadableEqualityComparer: IEqualityComparer<IDownloadable>
    {
        public bool Equals(IDownloadable x, IDownloadable y)
        {
            return x.Url == y.Url;
        }

        public int GetHashCode(IDownloadable obj)
        {
            return obj.GetHashCode();
        }
    }
}