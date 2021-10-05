namespace BMM.Api.Implementation.Models
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