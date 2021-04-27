using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BMM.Core.Implementations.Connection;
using BMM.Core.Implementations.DownloadManager;
using Foundation;
using UIKit;

namespace BMM.UI.iOS.DownloadManager
{
    /// <summary>
    ///     The iOS implementation of the download manager.
    /// </summary>
    public class IosDownloadManager : IDownloadManager
    {
        /// <summary>
        ///     Set the background session completion handler.
        ///     @see:
        ///     https://developer.xamarin.com/guides/ios/application_fundamentals/backgrounding/part_4_ios_backgrounding_walkthroughs/background_transfer_walkthrough/#Handling_Transfer_Completion
        /// </summary>
        public static Action BackgroundSessionCompletionHandler;

        private readonly INetworkSettings _networkSettings;
        private readonly bool _avoidDiscretionaryDownloadInBackground;

        private readonly NSUrlSession _backgroundSession;

        private readonly IList<IDownloadFile> _queue;

        private readonly NSUrlSession _session;

        public Func<IDownloadFile, string> PathNameForDownloadedFile { get; set; }

        public IEnumerable<IDownloadFile> Queue
        {
            get
            {
                lock (_queue)
                {
                    return _queue.ToList();
                }
            }
        }

        private string _identifier => NSBundle.MainBundle.BundleIdentifier + ".BackgroundTransferSession";

        /// <summary>
        /// </summary>
        /// <param name="sessionDownloadDelegate">
        ///     Set the background session completion handler.
        ///     @see:
        ///     https://developer.xamarin.com/guides/ios/application_fundamentals/backgrounding/part_4_ios_backgrounding_walkthroughs/background_transfer_walkthrough/#Handling_Transfer_Completion
        /// </param>
        /// <param name="avoidDiscretionaryDownloadInBackground">
        ///     Whether you should use a normal download session configuration instead of as background download session
        ///     configuration when the app is in the background to avoid the discretionary.
        ///     This makes the app download in the same process to be able to download immediately instead of waiting for the
        ///     systems scheduling algorithm.
        ///     The download will however not continue if the app is suspended while downloading.
        ///     @see
        ///     https://developer.apple.com/documentation/foundation/nsurlsessionconfiguration/1411552-discretionary?language=objc
        /// </param>
        public IosDownloadManager(UrlSessionDownloadDelegate sessionDownloadDelegate,
            INetworkSettings networkSettings,
            bool avoidDiscretionaryDownloadInBackground = true)
        {
            _networkSettings = networkSettings;
            _avoidDiscretionaryDownloadInBackground = avoidDiscretionaryDownloadInBackground;
            _queue = new List<IDownloadFile>();

            if (avoidDiscretionaryDownloadInBackground)
            {
                _session = InitDefaultSession(sessionDownloadDelegate);
            }

            _backgroundSession = InitBackgroundSession(sessionDownloadDelegate);

            ReinitializeTasksStartedBeforeAppTermination();
        }

        /// <summary>
        /// We see in the console that the download tasks try to resume at app start (without even calling this method) but they fail.
        /// The theory is that there is no authorization header yet registered.
        /// That's why this method does the job again - restarts uncompleted downloadTasks after we have all pending tasks ready.
        /// ToDo: Find the setting to disable the downloadTasks to resume automatically at app start because we do that in this method
        /// </summary>
        private void ReinitializeTasksStartedBeforeAppTermination()
        {
            _backgroundSession.GetTasks2(async (dataTasks, uploadTasks, downloadTasks) =>
            {
                bool mobileNetworkAllowed = await _networkSettings.GetMobileNetworkDownloadAllowed();
                foreach (var task in downloadTasks)
                {
                    Start(new DownloadFileImplementation(task), mobileNetworkAllowed);
                }
            });
        }

        public void Abort(IDownloadFile i)
        {
            var file = (DownloadFileImplementation)i;

            file.Status = DownloadFileStatus.CANCELED;
            file.Task?.Cancel();

            RemoveFile(file);
        }

        public void AbortAll()
        {
            foreach (var file in Queue)
            {
                Abort(file);
            }
        }

        public IDownloadFile CreateDownloadFile(string url)
        {
            return CreateDownloadFile(url, new Dictionary<string, string>());
        }

        public IDownloadFile CreateDownloadFile(string url, IDictionary<string, string> headers)
        {
            return new DownloadFileImplementation(url, headers);
        }

        public void Start(IDownloadFile i, bool mobileNetworkAllowed = true)
        {
            var file = (DownloadFileImplementation)i;

            AddFile(file);

            NSOperationQueue.MainQueue.BeginInvokeOnMainThread(() =>
            {
                NSUrlSession session;

                var inBackground = UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background;

                if (_avoidDiscretionaryDownloadInBackground && inBackground)
                {
                    session = _session;
                }
                else
                {
                    session = _backgroundSession;
                }

                file.StartDownload(session, mobileNetworkAllowed);
            });
        }

        private NSUrlSession createSession(NSUrlSessionConfiguration configuration, UrlSessionDownloadDelegate sessionDownloadDelegate)
        {
            configuration.HttpMaximumConnectionsPerHost = 1;

            return NSUrlSession.FromConfiguration(configuration, sessionDownloadDelegate, null);
        }

        /**
         * We initialize the background session with the following options
         * - nil as queue: The method, called on events could end up on any thread
         * - Only one connection per host
         */
        private NSUrlSession InitBackgroundSession(UrlSessionDownloadDelegate sessionDownloadDelegate)
        {
            sessionDownloadDelegate.Controller = this;
            var configuration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(_identifier);
            return InitSession(sessionDownloadDelegate, configuration);
        }

        private NSUrlSession InitDefaultSession(UrlSessionDownloadDelegate sessionDownloadDelegate)
        {
            return InitSession(sessionDownloadDelegate, NSUrlSessionConfiguration.DefaultSessionConfiguration);
        }

        private NSUrlSession InitSession(UrlSessionDownloadDelegate sessionDownloadDelegate,
            NSUrlSessionConfiguration configuration)
        {
            sessionDownloadDelegate.Controller = this;

            using (configuration)
            {
                return createSession(configuration, sessionDownloadDelegate);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected internal void AddFile(IDownloadFile file)
        {
            lock (_queue)
            {
                _queue.Add(file);
            }

            CollectionChanged?.Invoke(Queue, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, file));
        }

        protected internal void RemoveFile(IDownloadFile file)
        {
            lock (_queue)
            {
                _queue.Remove(file);
            }

            CollectionChanged?.Invoke(Queue, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, file));
        }
    }
}