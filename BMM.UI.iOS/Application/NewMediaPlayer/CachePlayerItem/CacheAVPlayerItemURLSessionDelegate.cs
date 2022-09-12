using System;
using Foundation;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public class CacheAVPlayerItemURLSessionDelegate : NSObject, INSUrlSessionDataDelegate
    {
        public Action<NSUrlSession, NSUrlSessionDataTask, NSData> ReceivedData { get; set; }
        public Action<NSUrlSession, NSUrlSessionDataTask, NSUrlResponse> ReceivedResponse { get; set; }
        public Action<NSUrlSession, NSUrlSessionTask, NSError> CompletedWithError { get; set; }
        
        [Export("URLSession:dataTask:didReceiveData:")]
        public void DidReceiveData(NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
        {
            ReceivedData?.Invoke(session, dataTask, data);
        }

        [Export("URLSession:dataTask:didReceiveResponse:completionHandler:")]
        public void DidReceiveResponse(NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
        {
            completionHandler(NSUrlSessionResponseDisposition.Allow);
            ReceivedResponse?.Invoke(session, dataTask, response);
        }

        [Export("URLSession:task:didCompleteWithError:")]
        public void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        {
            CompletedWithError?.Invoke(session, task, error);
        }
    }
}