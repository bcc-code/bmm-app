using BMM.Api.Framework.JsonConverter;
using BMM.Api.Implementation.Models;
using Newtonsoft.Json;

namespace BMM.Core.Implementations.Serialization
{
    public class DocumentSerializer: IDocumentSerializer
    {
        public string SerializeDocument(Document document)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new UnderscoreMappingResolver()
            };

            return JsonConvert.SerializeObject(document, Formatting.Indented, serializerSettings);
        }
    }
}