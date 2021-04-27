using System.Threading.Tasks;

namespace BMM.Core.Implementations.Downloading
{
    public interface IGlobalMediaDownloader
    {
        Task SynchronizeOfflineTracks();

        Task InitializeCacheAndSynchronizeTracks();
    }
}