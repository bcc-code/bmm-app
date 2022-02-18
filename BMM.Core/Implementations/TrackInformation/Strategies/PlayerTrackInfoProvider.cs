using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;

namespace BMM.Core.Implementations.TrackInformation.Strategies
{
    public class PlayerTrackInfoProvider : ITrackInfoProvider
    {
        public TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture)
        {
            if (track == null)
                return default;

            var subtitleList = new List<string>();
            
            string songNumber = track.Relations?.OfType<TrackRelationSongbook>()
                .Select(song => song.ShortName)
                .FirstOrDefault();
            
            subtitleList.AddIfNotNullOrEmpty(songNumber);

            if (!IsArtistUsedAsTitle(track))
                subtitleList.AddIfNotNullOrEmpty(track.Artist);
            
            subtitleList.AddIfNotNullOrEmpty(track.Album);

            return new TrackInformation
            {
                Label = GetTitle(track),
                Subtitle = string.Join($" {StringConstants.Dash} ", subtitleList)
            };
        }

        private static bool IsArtistUsedAsTitle(ITrackModel trackModel) => trackModel.Title == trackModel.Artist;
        private static string GetTitle(ITrackModel trackModel)
        {
            return string.IsNullOrEmpty(trackModel.Title)
                ? trackModel.Artist
                : trackModel.Title;
        }
    }
}