using System.Globalization;
using BMM.Api.Abstraction;

namespace BMM.Core.Implementations.TrackInformation.Strategies
{
    public interface ITrackInfoProvider
    {
        TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture);
    }

    public interface ISpecificTrackInfoProvider
    {
        bool HasSpecificStyling(ITrackModel track);

        TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture, TrackInformation defaultTrack);
    }
}