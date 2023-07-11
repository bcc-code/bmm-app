using System;
using BMM.Api.Implementation.Models;
using Newtonsoft.Json.Linq;

namespace BMM.Api.Framework.JsonConverter
{
    public class DocumentConverter : JsonCreationConverter<Document>
    {
        protected override Document Create(Type objectType, JObject jObject)
        {
            // First check up if the objectType is already fixed to a implementation.
            if (objectType != typeof(Document) && CanConvert(objectType))
            {
                return (Document)Activator.CreateInstance(objectType);
            }

            // A type has to be defined for type-juggling.
            var typeProperty = jObject["type"] ?? jObject["Type"];
            if (typeProperty == null)
            {
                throw new NotImplementedException("The type of the document has not been defined!");
            }

            // Try to find out what the implementation is.
            switch (typeProperty.Value<string>())
            {
                case "album":
                    return new Album();

                case "contributor":
                    return new Contributor();

                case "track":
                    return new Track();

                case "track_collection":
                    return new TrackCollection();

                case "podcast":
                    return new Podcast();

                case "section_header":
                    return new DiscoverSectionHeader();

                case "playlist":
                    return new Playlist();

                case "listening_streak":
                    return new ListeningStreak();

                case "chapter_header":
                    return new ChapterHeader();

                case "InfoMessage":
                    return new InfoMessage();

                case "Tile":
                    return new ContinueListeningTile();
                
                case "year_in_review":
                    return new YearInReviewTeaser();
                
                case "top_songs_collection":
                    return new TopSongsCollection();
                
                case "tile_message":
                    return new MessageTile();
                
                case "tile_video":
                    return new VideoTile();
                
                case "recommendation":
                    return new Recommendation();
            }

            return new UnsupportedDocument();
        }
    }
}