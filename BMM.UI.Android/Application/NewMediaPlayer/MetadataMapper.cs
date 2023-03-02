using Android.Support.V4.Media;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.NewMediaPlayer.Abstractions;
using BMM.UI.Droid.Application.Extensions;
using Com.Google.Android.Exoplayer2;
using Microsoft.IdentityModel.Tokens;

namespace BMM.UI.Droid.Application.NewMediaPlayer
{
    public interface IMetadataMapper
    {
        MediaDescriptionCompat FromTrack(IMediaTrack track);
        MediaDescriptionCompat FromBundle(Bundle bundle); 
        MediaItem ToMediaItem(MediaDescriptionCompat mediaDescriptionCompat);
        ITrackModel LookupTrackFromMetadata(MediaMetadataCompat metadata, IMediaQueue mediaQueue);
    }

    public class MetadataMapper : IMetadataMapper
    {
        private readonly IMediaQueue _mediaQueue;
        private const string MetadataNamespace = "org.brunstad.bmm.metadata.";
        public const string MetadataKeySubtype = MetadataNamespace + "SUBTYPE";
        public const string MetadataKeyLocalPath = MetadataNamespace + "LOCAL_PATH";

        public MetadataMapper(IMediaQueue mediaQueue)
        {
            _mediaQueue = mediaQueue;
        }
        
        public MediaDescriptionCompat FromTrack(IMediaTrack track)
        {
            var metadata = new MediaMetadataCompat.Builder()
                .PutString(MediaMetadataCompat.MetadataKeyMediaId, track.Id.ToString())
                .PutString(MediaMetadataCompat.MetadataKeyAlbum, track.Metadata.Album)
                .PutString(MediaMetadataCompat.MetadataKeyArtist, track.Metadata.Artist)
                .PutLong(MediaMetadataCompat.MetadataKeyDuration, track.Duration)
                .PutString(MediaMetadataCompat.MetadataKeyArtUri, track.ArtworkUri)
                .PutString(MediaMetadataCompat.MetadataKeyTitle, track.Metadata.Title)
                .PutString(MediaMetadataCompat.MetadataKeyDisplayTitle, track.Metadata.Title)
                .PutString(MediaMetadataCompat.MetadataKeyDisplaySubtitle, track.Metadata.Artist ?? "blank subtitle")
                .PutString(MediaMetadataCompat.MetadataKeyDisplayDescription, track.Metadata.Album ?? "blank album")
                .PutString(MetadataKeySubtype, track.Subtype.ToString())
                .PutString(MediaMetadataCompat.MetadataKeyMediaUri, track.Url)
                .PutString(MetadataKeyLocalPath, track.LocalPath)

                // Add downloadStatus to force the creation of an "extras" bundle in the resulting
                // MediaMetadataCompat object. This is needed to send accurate metadata to the media session during updates.
                .PutLong(
                    MediaMetadataCompat.MetadataKeyDownloadStatus,
                    track.Availability == ResourceAvailability.Remote ? MediaDescriptionCompat.StatusNotDownloaded : MediaDescriptionCompat.StatusDownloaded)
                .Build();

            var description = metadata.Description;
            description.Extras.PutAll(metadata.Bundle);

            return description;
        }
        
        
        public MediaDescriptionCompat FromBundle(Bundle bundle)
        {
            var metadata = new MediaMetadataCompat.Builder()
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyMediaId)
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyAlbum)
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyArtist)
                .AddLongFromBundle(bundle, MediaMetadataCompat.MetadataKeyDuration)
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyArtUri)
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyTitle)
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyDisplayTitle)
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyDisplaySubtitle)
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyDisplayDescription)
                .AddStringFromBundle(bundle, MediaMetadataCompat.MetadataKeyMediaUri)
                .AddStringFromBundle(bundle, MetadataKeyLocalPath)
                .Build();
            
            return metadata!.Description;
        }

        public MediaItem ToMediaItem(MediaDescriptionCompat mediaDescriptionCompat)
        {
            var id = mediaDescriptionCompat.Extras.GetString(MediaMetadataCompat.MetadataKeyMediaId);
            var uri = mediaDescriptionCompat.Extras.GetString(MediaMetadataCompat.MetadataKeyMediaUri);
            var localUri = mediaDescriptionCompat.Extras.GetString(MetadataKeyLocalPath);
            var album = mediaDescriptionCompat.Extras.GetString(MediaMetadataCompat.MetadataKeyAlbum);
            var artist = mediaDescriptionCompat.Extras.GetString(MediaMetadataCompat.MetadataKeyArtist);
            var title = mediaDescriptionCompat.Extras.GetString(MediaMetadataCompat.MetadataKeyTitle);

            string properUri = string.IsNullOrEmpty(localUri)
                ? uri
                : localUri;
            
            if (string.IsNullOrEmpty(id))
                return default;
            
            var mediaMetadata = new MediaMetadata.Builder()
                !.SetAlbumTitle(album)
                !.SetArtist(artist)
                !.SetTitle(title)
                !.SetExtras(mediaDescriptionCompat.Extras)
                !.Build();

            var mediaItemBuilder = new MediaItem.Builder()
                !.SetMediaId(id)
                !.SetUri(properUri)
                !.SetMediaMetadata(mediaMetadata);

            return mediaItemBuilder!.Build();
        }

        public ITrackModel LookupTrackFromMetadata(MediaMetadataCompat metadata, IMediaQueue mediaQueue)
        {
            var id = metadata.GetString(MediaMetadataCompat.MetadataKeyMediaId);

            if (string.IsNullOrEmpty(id))
                return null;

            return mediaQueue.GetTrackById(Convert.ToInt32(id));
        }
    }
}