using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.Serialization
{
    public interface IDocumentSerializer
    {
        string SerializeDocument(Document document);
    }
}