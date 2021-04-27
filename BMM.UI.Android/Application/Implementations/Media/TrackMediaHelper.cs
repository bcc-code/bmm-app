namespace BMM.UI.Droid.Application.Implementations.Media
{
    public class TrackMediaHelper : BMM.Core.Helpers.TrackMediaHelper
    {
        protected override string[] SupportedMimeTypes => new string[] {
            "audio/mpeg",
        };
    }
}