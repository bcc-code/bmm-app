using Android.Support.V4.Media;

namespace BMM.UI.Droid.Application.Extensions;

public static class MetadataBuilderExtensions
{
    public static MediaMetadataCompat.Builder AddStringFromBundle(this MediaMetadataCompat.Builder builder, Bundle bundle, string key)
    {
        return builder.PutString(key, bundle.GetString(key));
    }
    
    public static MediaMetadataCompat.Builder AddLongFromBundle(this MediaMetadataCompat.Builder builder, Bundle bundle, string key)
    {
        return builder.PutLong(key, bundle.GetLong(key));
    }
}