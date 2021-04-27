using System;
using Android.Support.V4.Media;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.NewMediaPlayer.Abstractions;

namespace BMM.UI.Droid.Application.NewMediaPlayer
{
    public interface IMetadataMapper
    {
        MediaDescriptionCompat BuildMediaDescription(IMediaTrack track);

        ITrackModel LookupTrackFromMetadata(MediaMetadataCompat metadata, IMediaQueue mediaQueue);
    }

    public class MetadataMapper : IMetadataMapper
    {
        private const string MetadataNamespace = "org.brunstad.bmm.metadata.";

        public const string MetadataKeySubtype = MetadataNamespace + "SUBTYPE";

        public const string MetadataKeyLocalPath = MetadataNamespace + "LOCAL_PATH";

        public MediaDescriptionCompat BuildMediaDescription(IMediaTrack track)
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

        public ITrackModel LookupTrackFromMetadata(MediaMetadataCompat metadata, IMediaQueue mediaQueue)
        {
            var id = metadata.GetString(MediaMetadataCompat.MetadataKeyMediaId);

            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return mediaQueue.GetTrackById(Convert.ToInt32(id));
        }
    }
}