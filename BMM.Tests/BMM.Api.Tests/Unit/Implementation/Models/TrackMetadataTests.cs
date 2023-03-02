using BMM.Api.Implementation.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BMM.Api.Test.Unit.Implementation.Models
{
    [TestFixture]
    public class TrackMetadataTests
    {
        [Test]
        // When the user requests an album from the API, the API doesn't send the album itself again for every track in that listing of inherited tracks/albums
        public void Deserialize_ParentAndRootParentNull()
        {
            var obj = JsonConvert.DeserializeObject<TrackMetadata>("{ parent: null, rootParent: null }");

            AssertEx.PropertyValuesAreEquals(obj, new TrackMetadata());
        }
    }
}

