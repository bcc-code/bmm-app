using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.DocumentFilters
{
    public interface IDocumentFilter
    {
        bool WherePredicate(Document document);
    }
}
