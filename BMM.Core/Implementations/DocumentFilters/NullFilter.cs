using BMM.Api.Implementation.Models;

namespace BMM.Core.Implementations.DocumentFilters
{
    public class NullFilter : IDocumentFilter
    {
        public bool WherePredicate(Document document)
        {
            return true;
        }
    }
}