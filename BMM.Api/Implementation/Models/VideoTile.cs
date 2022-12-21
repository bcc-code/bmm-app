namespace BMM.Api.Implementation.Models
{
    public class VideoTile : Document
    {
        public VideoTile()
        {
            DocumentType = DocumentType.TileVideo;
        }

        public string Header { get; set; }
        public string ButtonText { get; set; }
        public string ButtonUrl { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string VideoFileName { get; set; }
    }
}