using Newtonsoft.Json;

namespace BMM.Api.Implementation.Models
{
    [JsonObject]
    public class Contributor : CoverDocument
    {
        public Contributor()
        {
            DocumentType = DocumentType.Contributor;
        }

        public bool IsVisible { get; set; }

        public string Name { get; set; }
    }
}