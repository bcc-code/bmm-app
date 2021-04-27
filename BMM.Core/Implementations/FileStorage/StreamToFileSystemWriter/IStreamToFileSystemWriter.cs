using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.FileStorage.StreamToFileSystemWriter
{
    public interface IStreamToFileSystemWriter
    {
       Task WriteStreamForTrackMediaFile(Stream stream, CancellationToken cancellationToken, string filePath);
    }
}