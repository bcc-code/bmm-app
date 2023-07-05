namespace BMM.Api.Implementation.Models;

public class Recommendation : Document
{
    public Recommendation()
    {
        DocumentType = DocumentType.Contributor;
    }
    
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public Contributor? Contributor { get; set; }
    public Track? Track { get; set; }
    public Playlist? Playlist { get; set; }
    public Album? Album { get; set; }
}