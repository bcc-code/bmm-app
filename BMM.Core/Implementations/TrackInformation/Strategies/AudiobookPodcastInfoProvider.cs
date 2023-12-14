using System.Globalization;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Core.Constants;
using BMM.Core.Helpers;
using BMM.Core.ViewModels;

namespace BMM.Core.Implementations.TrackInformation.Strategies
{
    public class AudiobookPodcastInfoProvider : ITrackInfoProvider, ISpecificTrackInfoProvider
    {
        private readonly ITrackInfoProvider _defaultInfoProvider;

        public AudiobookPodcastInfoProvider(ITrackInfoProvider defaultInfoProvider)
        {
            _defaultInfoProvider = defaultInfoProvider;
        }

        public TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture)
        {
            var defaultTrack = _defaultInfoProvider.GetTrackInformation(track, culture);

            if (HasSpecificStyling(track))
                return GetTrackInformation(track, culture, defaultTrack);
            return defaultTrack;
        }

        public bool HasSpecificStyling(ITrackModel track)
        {
            return track.Tags.Contains(PodcastsConstants.FromKaareTagName)
                   || track.Tags.Contains(PodcastsConstants.ForbildeTagName)
                   || track.Tags.Contains(PodcastsConstants.RomanPodcastTagName)
                   || track.Tags.Contains(AslaksenConstants.HebrewTagName)
                   || track.Tags.Contains(AslaksenConstants.AsklaksenTagName);
        }

        public TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture, TrackInformation defaultTrack)
        {
            // We want to split the weekday from the rest of the pattern. Technically it would be better to set the order dynamically in case there is a language that commonly puts the weekday after the date.
            var patternWithoutWeekday = culture.DateTimeFormat.LongDatePattern.Replace("dddd", "").Trim().Trim(',').Trim();

            if (track.Tags.Contains(AslaksenConstants.AsklaksenTagName))
            {
                return new TrackInformation
                {
                    Label = defaultTrack.Label,
                    Subtitle = defaultTrack.Subtitle,
                    Meta = track.RecordedAt.ToNorwegianTime().ToString(culture.DateTimeFormat.LongDatePattern)
                };
            }
            return new TrackInformation
            {
                Label = defaultTrack.Label,
                Subtitle = track.RecordedAt.ToNorwegianTime().ToString("dddd", culture),
                Meta = track.RecordedAt.ToNorwegianTime().ToString(patternWithoutWeekday, culture)
            };
        }
    }
}