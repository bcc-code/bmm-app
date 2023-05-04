namespace BMM.Api.Implementation.Models;

public class HighlightedTextTrack : Document
{
    public HighlightedTextTrack(
        Track track,
        IList<SearchHighlight> searchHighlights)
    {
        Track = track;
        SearchHighlights = searchHighlights;
        DocumentType = DocumentType.HighlightedTextTrack;
    }
    
    public Track Track { get; }
    public IList<SearchHighlight> SearchHighlights { get; }
}