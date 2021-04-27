using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BMM.Api.Abstraction;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.TrackInformation.Strategies
{
    public class TypeKnownTrackInfoProvider : ITrackInfoProvider
    {
        public TrackInformation GetTrackInformation(ITrackModel track, CultureInfo culture)
        {
            //ToDo: we should get rid of the "smartness" in the Api that puts the artist in the title

            string songNumber = null;
            if (track.Subtype == TrackSubType.Song || track.Subtype == TrackSubType.Singsong)
            {
                songNumber = track.Relations?.OfType<TrackRelationSongbook>()
                    .Select(song => song.ShortName)
                    .FirstOrDefault();
            }

            var prioritizedList = new List<string>();
            if (songNumber == null || songNumber != track.Title)
                prioritizedList.Add(track.Title);

            if (track.Artist != track.Title)
                prioritizedList.Add(track.Artist);

            prioritizedList = prioritizedList.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            if (!prioritizedList.Any())
                prioritizedList.Add(string.Empty);

            if (songNumber != null)
                prioritizedList.Insert(1, songNumber);

            if (prioritizedList.Count < 2 && track.Meta.Parent.Title != track.Meta.RootParent.Title)
            {
                prioritizedList.Add(track.Meta.Parent.Title);
                prioritizedList.Add(track.Meta.RootParent.Title);
            }
            else
            {
                prioritizedList.Add(track.Album);
            }

            return new TrackInformation
            {
                Label = prioritizedList[0],
                Subtitle = prioritizedList.Count > 1 ? prioritizedList[1] : null,
                Meta = prioritizedList.Count > 2 ? prioritizedList[2] : null
            };
        }
    }
}