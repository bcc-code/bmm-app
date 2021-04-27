using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akavache;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using BMM.Core.Implementations.FileStorage;
using BMM.Core.Implementations.FileStorage.StreamToFileSystemWriter;

namespace BMM.Core.Implementations.Downloading.FileDownloader
{
    public class HttpClientFileDownloader : IFileDownloader
    {
        private readonly HttpClient _httpClient;
        private readonly IStreamToFileSystemWriter _streamToFileSystemWriter;
        private readonly IStorageManager _storageManager;
        private readonly IBlobCache _localStorage;
        private readonly IMediaRequestHttpHeaders _headerProvider;

        private CancellationTokenSource _cancellationTokenSource;
        private IDownloadable _currentDownloadable;

        public HttpClientFileDownloader(IStreamToFileSystemWriter streamToFileSystemWriter, IStorageManager storageManager, IBlobCache localStorage,
            IMediaRequestHttpHeaders headerProvider)
        {
            _streamToFileSystemWriter = streamToFileSystemWriter;
            _storageManager = storageManager;
            _localStorage = localStorage;
            _headerProvider = headerProvider;
            _httpClient = new HttpClient();
        }

        public async Task DownloadFile(IDownloadable downloadable)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            _currentDownloadable = downloadable;

            var filePath = _storageManager.SelectedStorage.GetUrlByFile(downloadable);
            var request = new HttpRequestMessage(HttpMethod.Get, downloadable.Url);
            foreach (var header in await _headerProvider.GetHeaders())
            {
                request.Headers.Add(header.Key, header.Value);
            }
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"The request returned with error HTTP status code {response.StatusCode}");

            try
            {
                // We store the currently downloading file to recover it in case the catch is never executed (e.g. kill the app)
                await _localStorage.InsertObject(StorageKeys.CurrentDownload, new PersistedDownloadable {Url = downloadable.Url, Id = downloadable.Id, Tags = downloadable.Tags});
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    await _streamToFileSystemWriter.WriteStreamForTrackMediaFile(contentStream, cancellationToken, filePath);
                }
            }
            catch
            {
                _storageManager.SelectedStorage.DeleteFile(downloadable);
                throw;
            }
            finally
            {
                await _localStorage.InsertObject<PersistedDownloadable>(StorageKeys.CurrentDownload, null);
                _cancellationTokenSource = null;
                _currentDownloadable = null;
            }
        }

        public void CancelDownload()
        {
            _cancellationTokenSource?.Cancel();
            if (_currentDownloadable != null)
            {
                _storageManager.SelectedStorage.DeleteFile(_currentDownloadable);
            }
        }
    }
}