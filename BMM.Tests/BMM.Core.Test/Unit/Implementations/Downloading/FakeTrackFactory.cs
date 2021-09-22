using System.Collections.Generic;
using BMM.Api.Implementation.Models;

namespace BMM.Core.Test.Unit.Implementations.Downloading
{
    public class FakeTrackFactory
    {
        public Track CreateTrackWithId(int id)
        {
            return new Track
            {
                Id = id,
                Language = "en",
                Media = new List<TrackMedia>
                {
                    new TrackMedia
                    {
                        Type = TrackMediaType.Audio,
                        Files = new List<TrackMediaFile>
                        {
                            new TrackMediaFile
                            {
                                Url = $"https://bmm-api.brunstad.org/file/protected/track/{id}/track_{id}_media_nb.mp3",
                                Duration = 9000,
                                Size = 10000
                            }
                        }
                    }
                }
            };
        }
    }
}