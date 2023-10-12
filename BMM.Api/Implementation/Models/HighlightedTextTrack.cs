namespace BMM.Api.Implementation.Models;

public class HighlightedTextTrack : Document
{
    public HighlightedTextTrack(Track track,
        IList<SearchHighlight> searchHighlights,
        int index)
    {
        Track = track;
        SearchHighlights = searchHighlights;
        DocumentType = DocumentType.HighlightedTextTrack;
        ItemIndex = index;
    }
    
    public Track Track { get; }
    public IList<SearchHighlight> SearchHighlights { get; }
    public int ItemIndex { get; }
}