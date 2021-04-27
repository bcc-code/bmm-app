namespace BMM.UI.iOS
{
    public class TrackMediaHelper : BMM.Core.Helpers.TrackMediaHelper
    {
        protected override string[] SupportedMimeTypes
        {
            get
            {
                return new string[] {
                    "audio/mpeg",
                };
            }
        }

        public TrackMediaHelper()
        {
        }
    }
}