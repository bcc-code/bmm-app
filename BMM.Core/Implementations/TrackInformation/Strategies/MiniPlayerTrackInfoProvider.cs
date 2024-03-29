﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BMM.Api.Abstraction;

namespace BMM.Core.Implementations.TrackInformation.Strategies
{
    public class MiniPlayerTrackInfoProvider : ITrackInfoProvider
    {
        public TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture)
        {
            if (track == null)
                return default;

            var list = new List<string>();
            if (track.Title != track.Artist)
                list.Add(track.Title);
            list.Add(track.Artist);
            list.Add(track.Album);

            list = list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            return new TrackInformation
            {
                Label = list[0],
                Subtitle = list.Count > 1 ? list[1] : string.Empty
            };
        }
    }
}