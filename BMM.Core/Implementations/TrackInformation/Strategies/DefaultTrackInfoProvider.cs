using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Helpers;
using MvvmCross.Localization;

namespace BMM.Core.Implementations.TrackInformation.Strategies
{
    public class DefaultTrackInfoProvider : ITrackInfoProvider
    {
        private IMvxLanguageBinder GlobalTextSource => new MvxLanguageBinder(GlobalConstants.GeneralNamespace, "Global");

        public TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture)
        {
            string label;
            string translatedSubtitle;
            string translatedMeta;

            if (track.Subtype == TrackSubType.Song || track.Subtype == TrackSubType.Singsong)
            {
                var songNumber = track.Relations?.OfType<TrackRelationSongbook>()
                    .Select(song => song.ShortName)
                    .FirstOrDefault();
                translatedSubtitle = songNumber ?? TranslatedType(track.Subtype);
            }
            else
            {
                translatedSubtitle = TranslatedType(track.Subtype);
            }

            translatedMeta = track.Album;
            var artist = track.Artist;
            var title = track.Title;
            if (string.IsNullOrWhiteSpace(title) || artist == title || (title == translatedSubtitle && !string.IsNullOrWhiteSpace(artist)))
                label = artist;
            else if (string.IsNullOrWhiteSpace(artist))
                label = title;
            else
            {
                label = title;
                translatedMeta = artist + (string.IsNullOrWhiteSpace(track.Album) ? "" : " · " + track.Album);
            }

            if (track.Tags.Contains("fra-kaare"))
            {
                /* Fra Kaare podcast album contains year e.g. "Fra Kaare 2018". We want to remove that year to meet design requirements.
                 * Please see podcast track design at https://app.zeplin.io/project/57bc6a9190d305660fd8e1ed/screen/5dd0468403f0d844cc632794 */
                translatedSubtitle = RemoveYear(track.Album);
                translatedMeta = track.RecordedAt.ToNorwegianTime().ToString(culture.DateTimeFormat.LongDatePattern, culture);
            }

            return new TrackInformation
            {
                Label = label,
                Subtitle = translatedSubtitle,
                Meta = translatedMeta
            };
        }

        public string TranslatedType(TrackSubType subtype)
        {
            string translationKey = subtype.ToString();
            if (subtype == TrackSubType.Singsong)
            {
                translationKey = TrackSubType.Song.ToString();
            }

            return GlobalTextSource.GetText(translationKey);
        }

        public string RemoveYear(string input)
        {
            return Regex.Replace(input, @"20\d\d", "");
        }
    }
}