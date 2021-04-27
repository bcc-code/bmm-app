using System.Threading.Tasks;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public interface IFileDownloader
    {
        Task DownloadFile(IDownloadable downloadable);

        void CancelDownload();
    }
}