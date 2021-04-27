using BMM.Api.Abstraction;
using BMM.Core.Implementations.Media;
using Foundation;
using MvvmCross;

namespace BMM.UI.iOS.NewMediaPlayer
{
    public static class MediaFileUrlHelper
    {
        public static NSUrl GetUrlFor(IMediaTrack mediaFile)
        {
            Mvx.IoCProvider.Resolve<MediaFileUrlSetter>().SetLocalPathIfDownloaded(mediaFile);
            var url = mediaFile.LocalPath != null ? new NSUrl(mediaFile.LocalPath, false) : new NSUrl(mediaFile.Url);
            return url;
        }
    }
}