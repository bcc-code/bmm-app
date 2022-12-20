namespace BMM.Api.Implementation.Models
{
    public class MessageTile : Document
    {
        public MessageTile()
        {
            DocumentType = DocumentType.TileMessage;
        }
        
        public string Header { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string CloseButtonText { get; set; }
    }
}