using BMM.Api.Implementation.Models;
using BMM.Core.Extensions;
using BMM.Core.Models.POs.Base.Interfaces;

namespace BMM.UI.iOS.Extensions;

public static class DocumentsExtensions
{
    public static Task<IDictionary<string, UIImage>>  DownloadCovers(this IEnumerable<Document> documents)
    {
        return documents
            .Select(x => x.GetCoverUrl())
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .DownloadAsImages();
    }
    
    public static Task<IDictionary<string, UIImage>>  DownloadCovers(this IEnumerable<IDocumentPO> documents)
    {
        return documents
            .Select(x => x.GetCoverUrl())
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .DownloadAsImages();
    }
}