using System;
using BMM.Api.Implementation.Models;
using Newtonsoft.Json.Linq;

namespace BMM.Api.Framework.JsonConverter
{
    public class TrackRelationConverter : JsonCreationConverter<TrackRelation>
    {
        protected override TrackRelation Create(Type objectType, JObject jObject)
        {
            // A type has to be defined for type-juggling.
            var typeProperty = jObject["type"] ?? jObject["Type"];
            if (typeProperty == null)
            {
                throw new NotImplementedException("The type of the track relation has not been defined!");
            }

            // Try to find out what the implementation is.
            switch (typeProperty.Value<string>())
            {
                case "bible":
                    return new TrackRelationBible();

                case "composer":
                    return new TrackRelationComposer();

                case "arranger":
                    return new TrackRelationArranger();

                case "lyricist":
                    return new TrackRelationLyricist();

                case "interpret":
                    return new TrackRelationInterpreter();

                case "songbook":
                    return new TrackRelationSongbook();

                case "external":
                    return new TrackRelationExternal();
            }

            return null;
        }
    }
}