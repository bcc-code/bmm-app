using BMM.Api.Implementation.Models;

namespace BMM.Core.Models
{
    public class ChapterHeader : Document
    {
        public ChapterHeader()
        {
            DocumentType = DocumentType.ChapterHeader;
        }

        public string Title { get; set; }
    }
}