using System;
using System.Threading.Tasks;

namespace BMM.UI.iOS.NewMediaPlayer.Interfaces
{
    public interface ICacheAVPlayerItemLoader
    {
        bool IsFullyDownloaded { get; }
        bool MoreThanAHalfFinished { get; }
        string UniqueKey { get; }
        
        /// <summary>
        ///     Notifies about finishing of download.
        ///     Bool parameter indicated if download was finished because of an error.
        /// </summary>
        public event EventHandler<bool> FinishedLoading;

        Task StartDataRequest(string url);
        void Cancel();
    }
}