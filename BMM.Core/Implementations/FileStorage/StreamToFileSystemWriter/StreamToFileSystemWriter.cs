using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BMM.Core.Implementations.FileStorage.StreamToFileSystemWriter
{
    public class StreamToFileSystemWriter : IStreamToFileSystemWriter
    {
        /// <summary>
        /// This is the default buffer size of the <see cref="Stream.CopyToAsync(System.IO.Stream)"/> method
        /// we have to redefine here because there is no overload
        /// with a cancellation token without to pass the buffer size
        /// </summary>
        private const int BufferSize = 81920;

        public async Task WriteStreamForTrackMediaFile(Stream stream, CancellationToken cancellationToken, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                await stream.CopyToAsync(fileStream, BufferSize, cancellationToken);
            }
        }
    }
}