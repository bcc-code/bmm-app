using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BMM.Core.Implementations.DownloadManager
{
    [Obsolete]
    public interface IDownloadManager
    {
        /// <summary>
        /// Gets the queue holding all the pending and downloading files.
        /// </summary>
        IEnumerable<IDownloadFile> Queue { get; }

        // ToDo: This event should be refactored and IMvxMessage should be used instead
        /// <summary>
        /// Occurs when the queue changed.
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// A function, returning the name of the path, where the download-file, given as argument, should be saved.
        /// </summary>
        Func<IDownloadFile, string> PathNameForDownloadedFile { get; set; }

        /// <summary>
        /// Creates a download file.
        /// </summary>
        IDownloadFile CreateDownloadFile(string url);

        /// <summary>
        /// Creates a download file.
        /// </summary>
        IDownloadFile CreateDownloadFile(string url, IDictionary<string, string> headers);

        /// <summary>
        /// Start downloading the file. Most of the systems will put this file into a queue first.
        /// </summary>
        void Start(IDownloadFile file, bool mobileNetworkAllowed = true);

        /// <summary>
        /// Abort downloading the file.
        /// </summary>
        void Abort(IDownloadFile file);

        /// <summary>
        /// Abort all.
        /// </summary>
        void AbortAll();
    }
}